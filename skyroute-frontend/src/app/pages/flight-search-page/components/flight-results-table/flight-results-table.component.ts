import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { FlightResultDto, SortDir, SortField } from '../../models/flight-search.models';
import { FlightFormatUtils } from '../../pipes/flight-format.pipe';

@Component({
  selector: 'app-flight-results-table',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section
      class="mt-[18px] rounded-[18px] border border-[color:var(--sr-border)] bg-white/[0.06] p-4 backdrop-blur-[10px]"
      aria-label="Resultados de vuelos"
    >
      <h2 class="mb-3 mt-0 text-lg font-extrabold">Resultados</h2>
      <div
        class="overflow-x-auto rounded-xl border border-[color:var(--sr-border)]"
        role="region"
        aria-label="Tabla de vuelos"
        tabindex="0"
      >
        <table class="w-full border-collapse text-sm">
          <thead>
            <tr>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
              >
                Proveedor
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
              >
                Vuelo
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
                [attr.aria-sort]="ariaSortAttr('departure')"
              >
                <button
                  class="inline-flex cursor-pointer items-center gap-1.5 bg-transparent font-extrabold text-[color:var(--sr-text-muted)] hover:text-[color:var(--sr-text)]"
                  type="button"
                  (click)="setSort('departure')"
                  [class.text-white]="sortField === 'departure'"
                >
                  Salida <span class="text-xs opacity-70">{{ sortIcon('departure') }}</span>
                </button>
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
              >
                Llegada
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
                [attr.aria-sort]="ariaSortAttr('duration')"
              >
                <button
                  class="inline-flex cursor-pointer items-center gap-1.5 bg-transparent font-extrabold text-[color:var(--sr-text-muted)] hover:text-[color:var(--sr-text)]"
                  type="button"
                  (click)="setSort('duration')"
                  [class.text-white]="sortField === 'duration'"
                >
                  Duración <span class="text-xs opacity-70">{{ sortIcon('duration') }}</span>
                </button>
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
              >
                Cabina
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
                [attr.aria-sort]="ariaSortAttr('price')"
              >
                <button
                  class="inline-flex cursor-pointer items-center gap-1.5 bg-transparent font-extrabold text-[color:var(--sr-text-muted)] hover:text-[color:var(--sr-text)]"
                  type="button"
                  (click)="setSort('price')"
                  [class.text-white]="sortField === 'price'"
                >
                  Precio <span class="text-xs opacity-70">{{ sortIcon('price') }}</span>
                </button>
              </th>
              <th
                class="whitespace-nowrap border-b border-white/[0.08] p-3.5 text-left font-extrabold text-[color:var(--sr-text-muted)]"
                scope="col"
              >
                Acción
              </th>
            </tr>
          </thead>
          <tbody>
            @for (r of sortedResults; track trackResult(0, r)) {
              <tr
                class="hover:bg-white/[0.04]"
                [style.background-color]="selectedFlight === r ? 'rgba(26,108,255,0.09)' : null"
              >
                <td class="border-b border-white/[0.08] p-3.5 text-left">{{ r.provider }}</td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">{{ r.flightNumber }}</td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">{{ formatTime(r.departureTime) }}</td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">
                  {{ formatTime(r.arrivalTime, r.departureTime) }}
                </td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">{{ formatDuration(r.durationMinutes) }}</td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">{{ r.cabinClass }}</td>
                <td class="whitespace-nowrap align-top border-b border-white/[0.08] p-3.5 text-left">
                  <span class="block font-bold">
                    <strong>{{ formatPrice(r.totalPrice, r.currency) }}</strong>
                    <span class="font-semibold opacity-90"> total</span>
                  </span>
                  <span class="mt-1 block text-[13px] font-semibold text-[color:var(--sr-text-muted)]">
                    {{ formatPrice(r.perPassengerPrice, r.currency) }}
                    <span class="font-semibold opacity-90"> per person</span>
                  </span>
                </td>
                <td class="border-b border-white/[0.08] p-3.5 text-left">
                  <button
                    class="cursor-pointer rounded-lg bg-[color:var(--sr-primary)] px-3 py-1.5 text-sm font-bold text-white hover:bg-[color:var(--sr-primary-hover)] disabled:cursor-not-allowed disabled:opacity-60"
                    type="button"
                    (click)="flightSelected.emit(r)"
                  >
                    Reservar
                  </button>
                </td>
              </tr>
            }
          </tbody>
        </table>
      </div>
    </section>
  `
})
export class FlightResultsTableComponent {
  @Input() results: FlightResultDto[] = [];
  @Input() selectedFlight: FlightResultDto | null = null;
  @Output() flightSelected = new EventEmitter<FlightResultDto>();

  sortField: SortField | null = null;
  sortDir: SortDir = 'asc';

  get sortedResults(): FlightResultDto[] {
    if (!this.sortField) return this.results;
    const field = this.sortField;
    const dir = this.sortDir === 'asc' ? 1 : -1;
    return [...this.results].sort((a, b) => {
      if (field === 'price') return (a.totalPrice - b.totalPrice) * dir;
      if (field === 'duration') return (a.durationMinutes - b.durationMinutes) * dir;
      return a.departureTime.localeCompare(b.departureTime) * dir;
    });
  }

  setSort(field: SortField): void {
    if (this.sortField === field) {
      this.sortDir = this.sortDir === 'asc' ? 'desc' : 'asc';
    } else {
      this.sortField = field;
      this.sortDir = 'asc';
    }
  }

  sortIcon(field: SortField): string {
    if (this.sortField !== field) return '⇅';
    return this.sortDir === 'asc' ? '▲' : '▼';
  }

  ariaSortAttr(field: SortField): string {
    if (this.sortField !== field) return 'none';
    return this.sortDir === 'asc' ? 'ascending' : 'descending';
  }

  trackResult(_index: number, r: FlightResultDto): string {
    return `${r.provider}|${r.flightNumber}|${r.departureTime}`;
  }

  formatTime(iso: string, compareWith?: string): string {
    return FlightFormatUtils.formatTime(iso, compareWith);
  }

  formatDuration(minutes: number): string {
    return FlightFormatUtils.formatDuration(minutes);
  }

  formatPrice(amount: number, currency: string): string {
    return FlightFormatUtils.formatPrice(amount, currency);
  }
}
