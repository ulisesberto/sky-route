import { NgFor, NgIf } from '@angular/common';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { finalize } from 'rxjs';

import { environment } from '../../../environments/environment';

@Component({
  selector: 'app-flight-search-page',
  imports: [ReactiveFormsModule, NgIf, NgFor],
  templateUrl: './flight-search-page.html',
  styleUrl: './flight-search-page.scss'
})
export class FlightSearchPage {
  private readonly fb = new FormBuilder();

  readonly airports: AirportOption[] = [
    { code: 'EZE', name: 'Buenos Aires (EZE)', city: 'Buenos Aires', countryCode: 'AR' },
    { code: 'AEP', name: 'Buenos Aires (AEP)', city: 'Buenos Aires', countryCode: 'AR' },
    { code: 'SCL', name: 'Santiago (SCL)', city: 'Santiago', countryCode: 'CL' },
    { code: 'LIM', name: 'Lima (LIM)', city: 'Lima', countryCode: 'PE' },
    { code: 'MIA', name: 'Miami (MIA)', city: 'Miami', countryCode: 'US' },
    { code: 'JFK', name: 'New York (JFK)', city: 'New York', countryCode: 'US' }
  ];

  readonly cabinClasses: CabinClass[] = ['Economy', 'Business', 'First'];

  readonly form = this.fb.nonNullable.group({
    origin: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    destination: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    departureDate: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    passengers: this.fb.nonNullable.control<number>(1, {
      validators: [Validators.required, Validators.min(1), Validators.max(9)]
    }),
    cabinClass: this.fb.nonNullable.control<CabinClass>('Economy', { validators: [Validators.required] })
  });

  isLoading = false;
  errorMessage: string | null = null;
  searchResults: FlightResultDto[] = [];
  hasCompletedSearch = false;

  constructor(private readonly http: HttpClient) {}

  swapRoute() {
    const origin = this.form.controls.origin.value;
    const destination = this.form.controls.destination.value;
    this.form.controls.origin.setValue(destination);
    this.form.controls.destination.setValue(origin);
    this.form.markAsDirty();
    this.form.updateValueAndValidity();
  }

  get canSubmit(): boolean {
    return !this.isLoading && this.form.valid && !this.hasSameRoute;
  }

  get hasSameRoute(): boolean {
    const o = this.form.controls.origin.value;
    const d = this.form.controls.destination.value;
    return !!o && !!d && o === d;
  }

  get showEmptyState(): boolean {
    return this.hasCompletedSearch && this.searchResults.length === 0;
  }

  get showResultsTable(): boolean {
    return this.hasCompletedSearch && this.searchResults.length > 0;
  }

  submit() {
    this.errorMessage = null;
    this.form.markAllAsTouched();
    if (!this.canSubmit) return;

    const payload: FlightSearchRequest = {
      origin: this.form.controls.origin.value,
      destination: this.form.controls.destination.value,
      departureDate: this.form.controls.departureDate.value,
      passengers: this.form.controls.passengers.value,
      cabinClass: this.form.controls.cabinClass.value
    };

    this.isLoading = true;
    this.searchResults = [];

    const url = this.flightSearchUrl();

    this.http
      .post<FlightSearchResponseDto>(url, payload)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: (body) => {
          this.hasCompletedSearch = true;
          this.searchResults = body.results ?? [];
        },
        error: (err: unknown) => {
          this.errorMessage = this.toUserError(err);
        }
      });
  }

  formatTime(iso: string): string {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '—';
    return new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit' }).format(d);
  }

  formatDuration(minutes: number): string {
    if (minutes < 0 || !Number.isFinite(minutes)) return '—';
    const h = Math.floor(minutes / 60);
    const m = minutes % 60;
    if (h <= 0) return `${m}m`;
    return m > 0 ? `${h}h ${m}m` : `${h}h`;
  }

  formatPrice(amount: number, currency: string): string {
    const c = currency || 'USD';
    return new Intl.NumberFormat(undefined, { style: 'currency', currency: c }).format(amount);
  }

  trackAirport(_index: number, a: AirportOption): string {
    return a.code;
  }

  trackCabinClass(_index: number, c: CabinClass): string {
    return c;
  }

  trackResult(_index: number, r: FlightResultDto): string {
    return `${r.provider}|${r.flightNumber}|${r.departureTime}`;
  }

  private flightSearchUrl(): string {
    const path = '/api/flights/search';
    const base = environment.apiBaseUrl.trim().replace(/\/+$/, '');
    return base ? `${base}${path}` : path;
  }

  private toUserError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 0) return 'No pudimos conectar con el backend. Verificá que esté corriendo.';
      if (err.status >= 500) return 'Ocurrió un error en el servidor. Intentá de nuevo.';
      if (err.status === 400) return 'Revisá los datos ingresados e intentá de nuevo.';
      return 'Ocurrió un error. Intentá de nuevo.';
    }
    return 'Ocurrió un error inesperado. Intentá de nuevo.';
  }
}

type CabinClass = 'Economy' | 'Business' | 'First';

type FlightSearchRequest = {
  origin: string;
  destination: string;
  departureDate: string;
  passengers: number;
  cabinClass: CabinClass;
};

type AirportOption = {
  code: string;
  name: string;
  city: string;
  countryCode: string;
};

type FlightSearchResponseDto = {
  results: FlightResultDto[];
};

type FlightResultDto = {
  provider: string;
  flightNumber: string;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabinClass: string;
  perPassengerPrice: number;
  totalPrice: number;
  currency: string;
  isInternational: boolean;
};
