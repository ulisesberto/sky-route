import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { FlightResultDto } from '../../../models/flight-search.models';
import { FlightFormatUtils } from '../../../pipes/flight-format.pipe';

@Component({
  selector: 'app-flight-summary',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="mb-4 rounded-xl border border-white/[0.1] bg-white/[0.04] p-4">
      <div class="mb-2 flex flex-wrap items-center gap-x-3 gap-y-1 text-base font-bold">
        <span>{{ flight.origin }}</span>
        <span class="text-[color:var(--sr-text-muted)]">→</span>
        <span>{{ flight.destination }}</span>
        @if (flight.isInternational) {
          <span
            class="rounded-full border border-[rgba(26,108,255,0.4)] bg-[rgba(26,108,255,0.12)] px-2 py-0.5 text-xs font-semibold text-[color:var(--sr-text-muted)]"
          >
            Internacional
          </span>
        }
      </div>
      <div class="grid grid-cols-2 gap-x-6 gap-y-1 text-sm text-[color:var(--sr-text-muted)] sm:grid-cols-4">
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Proveedor</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{ flight.provider }}</span>
        </div>
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Vuelo</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{ flight.flightNumber }}</span>
        </div>
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Salida</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{ formatTime(flight.departureTime) }}</span>
        </div>
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Llegada</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{
            formatTime(flight.arrivalTime, flight.departureTime)
          }}</span>
        </div>
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Cabina</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{ flight.cabinClass }}</span>
        </div>
        <div>
          <span class="block text-[11px] font-bold uppercase tracking-wider opacity-60">Duración</span>
          <span class="font-semibold text-[color:var(--sr-text)]">{{
            formatDuration(flight.durationMinutes)
          }}</span>
        </div>
      </div>
    </div>
  `
})
export class FlightSummaryComponent {
  @Input({ required: true }) flight!: FlightResultDto;

  formatTime(iso: string, compareWith?: string): string {
    return FlightFormatUtils.formatTime(iso, compareWith);
  }

  formatDuration(minutes: number): string {
    return FlightFormatUtils.formatDuration(minutes);
  }
}
