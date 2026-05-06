import { ChangeDetectionStrategy, Component, Input } from '@angular/core';

@Component({
  selector: 'app-booking-confirmation',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div
      class="mb-4 rounded-xl border border-[rgba(34,197,94,0.45)] bg-[rgba(34,197,94,0.1)] p-4"
      role="status"
    >
      <p class="mb-1 mt-0 font-extrabold text-[rgba(134,239,172,0.95)]">¡Reserva confirmada!</p>
      <p class="m-0 font-semibold text-[color:var(--sr-text-muted)]">
        Código de referencia:
        <span class="font-extrabold text-[color:var(--sr-text)]">{{ bookingReference }}</span>
      </p>
    </div>
  `
})
export class BookingConfirmationComponent {
  @Input({ required: true }) bookingReference!: string;
}
