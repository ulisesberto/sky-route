import {
  ChangeDetectionStrategy,
  Component,
  EventEmitter,
  inject,
  Input,
  OnChanges,
  Output,
  SimpleChanges
} from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { AirportOption, CabinClass, FlightSearchRequest } from '../../models/flight-search.models';

@Component({
  selector: 'app-flight-search-form',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  template: `
    <section
      class="rounded-[18px] border border-[color:var(--sr-border)] bg-white/[0.06] p-4 backdrop-blur-[10px]"
      aria-label="Búsqueda de vuelos"
    >
      <form class="m-0" [formGroup]="form" (ngSubmit)="submit()">
        <div
          class="grid items-end gap-3 [grid-template-columns:1.35fr_auto_1.35fr_1fr_0.75fr_0.9fr_auto] max-[980px]:grid-cols-1"
        >
          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-[color:var(--sr-text-muted)]" for="origin">De</label>
            <select
              id="origin"
              class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
              formControlName="origin"
              [class.border-red-500]="form.controls.origin.touched && form.controls.origin.invalid"
            >
              <option value="" disabled>Seleccioná un aeropuerto</option>
              @for (a of airports; track a.code) {
                <option [value]="a.code">{{ a.name }}</option>
              }
            </select>
            @if (form.controls.origin.touched && form.controls.origin.hasError('required')) {
              <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
                Seleccioná un origen
              </div>
            }
          </div>

          <div class="flex items-end justify-center pb-0.5 max-[980px]:justify-start max-[980px]:pb-0">
            <button
              class="h-11 w-11 cursor-pointer rounded-xl border border-[color:var(--sr-border)] bg-white/[0.06] font-black text-[color:var(--sr-text)] hover:bg-white/[0.12] disabled:cursor-not-allowed"
              type="button"
              (click)="swapRoute()"
              [disabled]="isLoading"
              aria-label="Intercambiar origen y destino"
              title="Intercambiar"
            >
              ⇄
            </button>
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-[color:var(--sr-text-muted)]" for="destination">A</label>
            <select
              id="destination"
              class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
              formControlName="destination"
              [class.border-red-500]="
                (form.controls.destination.touched && form.controls.destination.invalid) || hasSameRoute
              "
            >
              <option value="" disabled>Seleccioná un aeropuerto</option>
              @for (a of airports; track a.code) {
                <option [value]="a.code">{{ a.name }}</option>
              }
            </select>
            @if (hasSameRoute) {
              <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
                Origen y destino no pueden ser iguales
              </div>
            }
            @if (!hasSameRoute && form.controls.destination.touched && form.controls.destination.hasError('required')) {
              <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
                Seleccioná un destino
              </div>
            }
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-[color:var(--sr-text-muted)]" for="departureDate">Salida</label>
            <input
              id="departureDate"
              class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
              type="date"
              formControlName="departureDate"
              [class.border-red-500]="form.controls.departureDate.touched && form.controls.departureDate.invalid"
            />
            @if (form.controls.departureDate.touched && form.controls.departureDate.hasError('required')) {
              <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
                Elegí una fecha
              </div>
            }
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-[color:var(--sr-text-muted)]" for="passengers">Pasajeros</label>
            <input
              id="passengers"
              class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
              type="number"
              min="1"
              max="9"
              formControlName="passengers"
              [class.border-red-500]="form.controls.passengers.touched && form.controls.passengers.invalid"
            />
            @if (
              form.controls.passengers.touched &&
              (form.controls.passengers.hasError('min') ||
                form.controls.passengers.hasError('max') ||
                form.controls.passengers.hasError('required'))
            ) {
              <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
                Debe ser entre 1 y 9
              </div>
            }
          </div>

          <div class="flex flex-col gap-1.5">
            <label class="text-[13px] font-bold text-[color:var(--sr-text-muted)]" for="cabinClass">Cabina</label>
            <select
              id="cabinClass"
              class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
              formControlName="cabinClass"
            >
              @for (c of cabinClasses; track c) {
                <option [value]="c">{{ c }}</option>
              }
            </select>
          </div>

          <div class="flex items-end justify-end max-[980px]:justify-stretch">
            <button
              class="inline-flex min-h-11 cursor-pointer items-center gap-2.5 rounded-xl bg-[color:var(--sr-primary)] px-6 py-3.5 font-extrabold text-white hover:bg-[color:var(--sr-primary-hover)] disabled:cursor-not-allowed disabled:opacity-60 max-[980px]:w-full max-[980px]:justify-center"
              type="submit"
              [disabled]="!canSubmit"
            >
              @if (isLoading) {
                <span
                  class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-white/45 border-t-white"
                  aria-hidden="true"
                ></span>
                Buscando…
              }
              @if (!isLoading) {
                Buscar
              }
            </button>
          </div>
        </div>
      </form>
    </section>
  `
})
export class FlightSearchFormComponent implements OnChanges {
  private readonly fb = inject(FormBuilder);

  @Input() airports: AirportOption[] = [];
  @Input() cabinClasses: CabinClass[] = [];
  @Input() isLoading = false;
  @Output() searched = new EventEmitter<FlightSearchRequest>();

  readonly form = this.fb.nonNullable.group({
    origin: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    destination: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    departureDate: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    passengers: this.fb.nonNullable.control<number>(1, {
      validators: [Validators.required, Validators.min(1), Validators.max(9)]
    }),
    cabinClass: this.fb.nonNullable.control<CabinClass>('Economy', { validators: [Validators.required] })
  });

  get hasSameRoute(): boolean {
    const o = this.form.controls.origin.value;
    const d = this.form.controls.destination.value;
    return !!o && !!d && o === d;
  }

  get canSubmit(): boolean {
    return !this.isLoading && this.form.valid && !this.hasSameRoute;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['isLoading']) {
      if (this.isLoading) {
        this.form.disable();
      } else {
        this.form.enable();
      }
    }
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

  submit(): void {
    this.form.markAllAsTouched();
    if (!this.canSubmit) return;
    const payload: FlightSearchRequest = {
      origin: this.form.controls.origin.value,
      destination: this.form.controls.destination.value,
      departureDate: this.form.controls.departureDate.value,
      passengers: this.form.controls.passengers.value,
      cabinClass: this.form.controls.cabinClass.value
    };
    this.searched.emit(payload);
  }

}
