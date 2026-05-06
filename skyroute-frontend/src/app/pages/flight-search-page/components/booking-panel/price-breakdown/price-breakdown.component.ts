import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { FlightResultDto } from '../../../models/flight-search.models';
import { FlightFormatUtils } from '../../../pipes/flight-format.pipe';

@Component({
  selector: 'app-price-breakdown',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mb-5 rounded-xl border border-white/[0.1] bg-white/[0.04] p-4">
      <h3 class="mb-2 mt-0 text-sm font-extrabold uppercase tracking-wider text-[color:var(--sr-text-muted)]">
        Desglose de precio
      </h3>
      <div class="flex flex-wrap items-center gap-x-2 gap-y-1 text-sm">
        <span class="font-semibold text-[color:var(--sr-text-muted)]">
          {{ formatPrice(flight.perPassengerPrice, flight.currency) }} por persona
        </span>
        <span class="text-[color:var(--sr-text-muted)] opacity-60">×</span>
        <span class="font-semibold text-[color:var(--sr-text-muted)]">{{ passengers }} pasajero(s)</span>
        <span class="text-[color:var(--sr-text-muted)] opacity-60">=</span>
        <span class="text-base font-extrabold text-[color:var(--sr-text)]">
          {{ formatPrice(flight.totalPrice, flight.currency) }} total
        </span>
      </div>
    </div>
  `
})
export class PriceBreakdownComponent {
  @Input({ required: true }) flight!: FlightResultDto;
  @Input({ required: true }) passengers!: number;

  formatPrice(amount: number, currency: string): string {
    return FlightFormatUtils.formatPrice(amount, currency);
  }
}
