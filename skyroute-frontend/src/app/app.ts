import { Component } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-root',
  imports: [ReactiveFormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
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

    // Endpoint contract is defined by backend; for now we call a conventional route.
    const apiBaseUrl = (globalThis as any).__SR_API_BASE_URL__ as string | undefined;
    const url = `${apiBaseUrl ?? 'http://localhost:5000'}/api/flights/search`;

    this.http
      .post(url, payload)
      .pipe(finalize(() => (this.isLoading = false)))
      .subscribe({
        next: () => {
          // FE-2 (results) is intentionally deferred; this keeps FE-1 submit + loading done.
        },
        error: (err: unknown) => {
          this.errorMessage = this.toUserError(err);
        }
      });
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
