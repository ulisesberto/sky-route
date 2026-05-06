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

  readonly bookingForm = this.fb.nonNullable.group({
    fullName: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    email: this.fb.nonNullable.control<string>('', { validators: [Validators.required, Validators.email] }),
    documentNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] })
  });

  isLoading = false;
  errorMessage: string | null = null;
  searchResults: FlightResultDto[] = [];
  hasCompletedSearch = false;
  sortField: 'price' | 'duration' | 'departure' | null = null;
  sortDir: 'asc' | 'desc' = 'asc';

  selectedFlight: FlightResultDto | null = null;
  bookingIsLoading = false;
  bookingReference: string | null = null;
  bookingErrorMessage: string | null = null;

  constructor(private readonly http: HttpClient) {}

  get isInternational(): boolean {
    return this.selectedFlight?.isInternational ?? false;
  }

  get documentLabel(): string {
    return this.isInternational ? 'Passport Number' : 'National ID';
  }

  selectFlight(flight: FlightResultDto): void {
    this.selectedFlight = flight;
    this.bookingReference = null;
    this.bookingErrorMessage = null;
    this.bookingForm.reset();
    this.bookingForm.enable();
    this.updateDocumentValidators();
  }

  private updateDocumentValidators(): void {
    const docControl = this.bookingForm.controls.documentNumber;
    if (this.isInternational) {
      docControl.setValidators([Validators.required, Validators.pattern(/^[A-Z0-9]{6,9}$/)]);
    } else {
      docControl.setValidators([Validators.required, Validators.pattern(/^[0-9]{6,10}$/)]);
    }
    docControl.updateValueAndValidity();
  }

  get canConfirmBooking(): boolean {
    return !this.bookingIsLoading && this.bookingForm.valid && !this.bookingReference;
  }

  confirmBooking(): void {
    this.bookingForm.markAllAsTouched();
    if (!this.canConfirmBooking || !this.selectedFlight) return;

    const flight = this.selectedFlight;
    const formValue = this.bookingForm.getRawValue();

    const payload: BookingRequest = {
      flight: {
        provider: flight.provider,
        flightNumber: flight.flightNumber,
        origin: flight.origin,
        destination: flight.destination,
        departureTime: flight.departureTime,
        arrivalTime: flight.arrivalTime,
        cabinClass: flight.cabinClass,
        perPassengerPrice: flight.perPassengerPrice,
        totalPrice: flight.totalPrice,
        currency: flight.currency,
        isInternational: flight.isInternational
      },
      passengers: this.form.controls.passengers.value,
      passenger: {
        fullName: formValue.fullName,
        email: formValue.email,
        documentNumber: formValue.documentNumber
      }
    };

    this.bookingIsLoading = true;
    this.bookingErrorMessage = null;

    this.http
      .post<BookingResponse>(this.bookingUrl(), payload)
      .pipe(finalize(() => (this.bookingIsLoading = false)))
      .subscribe({
        next: (body) => {
          this.bookingReference = body.bookingReference;
          this.bookingForm.disable();
        },
        error: (err: unknown) => {
          this.bookingErrorMessage = this.toBookingError(err);
        }
      });
  }

  swapRoute(): void {
    const origin = this.form.controls.origin.value;
    const destination = this.form.controls.destination.value;
    this.form.controls.origin.setValue(destination);
    this.form.controls.destination.setValue(origin);
    this.form.markAsDirty();
    this.form.controls.origin.markAsTouched();
    this.form.controls.destination.markAsTouched();
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

  get sortedResults(): FlightResultDto[] {
    if (!this.sortField) return this.searchResults;
    const field = this.sortField;
    const dir = this.sortDir === 'asc' ? 1 : -1;
    return [...this.searchResults].sort((a, b) => {
      if (field === 'price') return (a.totalPrice - b.totalPrice) * dir;
      if (field === 'duration') return (a.durationMinutes - b.durationMinutes) * dir;
      return a.departureTime.localeCompare(b.departureTime) * dir;
    });
  }

  setSort(field: 'price' | 'duration' | 'departure'): void {
    if (this.sortField === field) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDir = 'asc';
    }
  }

  sortIcon(field: 'price' | 'duration' | 'departure'): string {
    if (this.sortField !== field) return '⇅';
    return this.sortDir === 'asc' ? '▲' : '▼';
  }

  ariaSortAttr(field: 'price' | 'duration' | 'departure'): string {
    if (this.sortField !== field) return 'none';
    return this.sortDir === 'asc' ? 'ascending' : 'descending';
  }

  submit(): void {
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
    this.hasCompletedSearch = false;
    this.sortField = null;
    this.sortDir = 'asc';
    this.selectedFlight = null;
    this.form.disable();

    this.http
      .post<FlightSearchResponseDto>(this.flightSearchUrl(), payload)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.form.enable();
        })
      )
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

  /**
   * Formats an ISO datetime as HH:MM.
   * When compareWith is provided and the dates differ, appends "+1d" to signal overnight arrival.
   */
  formatTime(iso: string, compareWith?: string): string {
    const d = new Date(iso);
    if (Number.isNaN(d.getTime())) return '—';
    const time = new Intl.DateTimeFormat(undefined, { hour: '2-digit', minute: '2-digit' }).format(d);
    if (compareWith) {
      const ref = new Date(compareWith);
      if (!Number.isNaN(ref.getTime()) && d.toDateString() !== ref.toDateString()) {
        return `${time} +1d`;
      }
    }
    return time;
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
    try {
      return new Intl.NumberFormat(undefined, { style: 'currency', currency: c }).format(amount);
    } catch {
      return `${amount} ${c}`;
    }
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

  private bookingUrl(): string {
    const path = '/api/bookings';
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

  private toBookingError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 0) return 'No pudimos conectar con el backend. Verificá que esté corriendo.';
      if (err.status >= 500) return 'Error en el servidor al confirmar la reserva. Intentá de nuevo.';
      if (err.status === 400) return 'Datos de reserva inválidos. Revisá el formulario e intentá de nuevo.';
      return 'No se pudo confirmar la reserva. Intentá de nuevo.';
    }
    return 'Error inesperado al confirmar la reserva.';
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

type FlightSnapshot = {
  provider: string;
  flightNumber: string;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  cabinClass: string;
  perPassengerPrice: number;
  totalPrice: number;
  currency: string;
  isInternational: boolean;
};

type PassengerData = {
  fullName: string;
  email: string;
  documentNumber: string;
};

type BookingRequest = {
  flight: FlightSnapshot;
  passengers: number;
  passenger: PassengerData;
};

type BookingResponse = {
  bookingReference: string;
};
