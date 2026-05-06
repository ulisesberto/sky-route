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
import { PassengerData } from '../../../models/flight-search.models';

@Component({
  selector: 'app-passenger-form',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [ReactiveFormsModule],
  template: `
    <form [formGroup]="bookingForm" (ngSubmit)="submit()">
      <div class="grid gap-4 sm:grid-cols-2">
        <div class="flex flex-col gap-1.5">
          <label
            class="text-[13px] font-bold text-[color:var(--sr-text-muted)]"
            for="fullName"
          >
            Nombre completo
          </label>
          <input
            id="fullName"
            class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
            type="text"
            formControlName="fullName"
            placeholder="Ej: Juan Pérez"
            autocomplete="name"
            [class.border-red-500]="
              bookingForm.controls.fullName.touched && bookingForm.controls.fullName.invalid
            "
          />
          @if (bookingForm.controls.fullName.touched && bookingForm.controls.fullName.hasError('required')) {
            <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
              Ingresá tu nombre completo
            </div>
          }
        </div>

        <div class="flex flex-col gap-1.5">
          <label
            class="text-[13px] font-bold text-[color:var(--sr-text-muted)]"
            for="bookingEmail"
          >
            Email
          </label>
          <input
            id="bookingEmail"
            class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60"
            type="email"
            formControlName="email"
            placeholder="Ej: juan@email.com"
            autocomplete="email"
            [class.border-red-500]="bookingForm.controls.email.touched && bookingForm.controls.email.invalid"
          />
          @if (bookingForm.controls.email.touched && bookingForm.controls.email.hasError('required')) {
            <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
              Ingresá tu email
            </div>
          }
          @if (
            bookingForm.controls.email.touched &&
            !bookingForm.controls.email.hasError('required') &&
            bookingForm.controls.email.hasError('email')
          ) {
            <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
              Ingresá un email válido
            </div>
          }
        </div>

        <div class="flex flex-col gap-1.5 sm:col-span-2">
          <label
            class="text-[13px] font-bold text-[color:var(--sr-text-muted)]"
            for="documentNumber"
          >
            {{ documentLabel }}
          </label>
          <input
            id="documentNumber"
            class="w-full rounded-xl border border-white/[0.18] bg-white/[0.92] px-3 py-3 text-[color:var(--sr-text-dark)] outline-none focus:border-[rgba(26,108,255,0.7)] focus:ring-4 focus:ring-[color:var(--sr-focus)] disabled:cursor-not-allowed disabled:opacity-60 sm:w-1/2"
            type="text"
            formControlName="documentNumber"
            [placeholder]="isInternational ? 'Ej: AB123456' : 'Ej: 12345678'"
            [class.border-red-500]="
              bookingForm.controls.documentNumber.touched &&
              bookingForm.controls.documentNumber.invalid
            "
          />
          @if (
            bookingForm.controls.documentNumber.touched &&
            bookingForm.controls.documentNumber.hasError('required')
          ) {
            <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
              Ingresá tu {{ documentLabel }}
            </div>
          }
          @if (
            bookingForm.controls.documentNumber.touched &&
            !bookingForm.controls.documentNumber.hasError('required') &&
            bookingForm.controls.documentNumber.hasError('pattern')
          ) {
            <div class="min-h-4 text-xs text-[rgba(255,170,170,0.95)]">
              @if (isInternational) {
                El pasaporte debe tener entre 6 y 9 caracteres alfanuméricos en mayúscula (ej: AB123456)
              }
              @if (!isInternational) {
                El DNI debe tener entre 6 y 10 dígitos numéricos
              }
            </div>
          }
          <div class="text-[11px] text-[color:var(--sr-text-muted)] opacity-70">
            @if (isInternational) {
              Vuelo internacional — se requiere Passport Number
            }
            @if (!isInternational) {
              Vuelo doméstico — se requiere National ID
            }
          </div>
        </div>
      </div>

      @if (errorMessage) {
        <div
          class="mt-4 rounded-xl border border-[rgba(255,77,79,0.55)] bg-[rgba(255,77,79,0.12)] p-3 font-semibold text-[rgba(255,214,214,0.95)]"
          role="alert"
        >
          {{ errorMessage }}
        </div>
      }

      <div class="mt-5 flex items-center gap-3">
        <button
          class="inline-flex min-h-11 cursor-pointer items-center gap-2.5 rounded-xl bg-[color:var(--sr-primary)] px-6 py-3.5 font-extrabold text-white hover:bg-[color:var(--sr-primary-hover)] disabled:cursor-not-allowed disabled:opacity-60"
          type="submit"
          [disabled]="!canConfirm"
        >
          @if (isLoading) {
            <span
              class="inline-block h-4 w-4 animate-spin rounded-full border-2 border-white/45 border-t-white"
              aria-hidden="true"
            ></span>
            Confirmando…
          }
          @if (!isLoading) {
            Confirmar reserva
          }
        </button>

        @if (errorMessage) {
          <button
            class="cursor-pointer rounded-xl border border-[color:var(--sr-border)] bg-white/[0.06] px-4 py-3 font-semibold text-[color:var(--sr-text)] hover:bg-white/[0.12] disabled:cursor-not-allowed disabled:opacity-60"
            type="button"
            (click)="retry()"
            [disabled]="isLoading"
          >
            Reintentar
          </button>
        }
      </div>
    </form>
  `
})
export class PassengerFormComponent implements OnChanges {
  private readonly fb = inject(FormBuilder);

  @Input({ required: true }) isInternational!: boolean;
  @Input() flightKey = '';
  @Input() isLoading = false;
  @Input() errorMessage: string | null = null;
  @Output() formSubmitted = new EventEmitter<PassengerData>();
  @Output() retried = new EventEmitter<void>();

  readonly bookingForm = this.fb.nonNullable.group({
    fullName: this.fb.nonNullable.control<string>('', { validators: [Validators.required] }),
    email: this.fb.nonNullable.control<string>('', { validators: [Validators.required, Validators.email] }),
    documentNumber: this.fb.nonNullable.control<string>('', { validators: [Validators.required] })
  });

  get documentLabel(): string {
    return this.isInternational ? 'Passport Number' : 'National ID';
  }

  get canConfirm(): boolean {
    return !this.isLoading && this.bookingForm.valid;
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['flightKey'] && !changes['flightKey'].firstChange) {
      this.bookingForm.reset();
    }
    if (changes['isInternational']) {
      this.updateDocumentValidators();
    }
    if (changes['isLoading']) {
      if (this.isLoading) {
        this.bookingForm.disable();
      } else {
        this.bookingForm.enable();
      }
    }
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

  submit(): void {
    this.bookingForm.markAllAsTouched();
    if (!this.canConfirm) return;
    const v = this.bookingForm.getRawValue();
    this.formSubmitted.emit({ fullName: v.fullName, email: v.email, documentNumber: v.documentNumber });
  }

  retry(): void {
    this.retried.emit();
  }
}
