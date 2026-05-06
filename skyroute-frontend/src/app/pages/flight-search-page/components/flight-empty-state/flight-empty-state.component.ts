import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-flight-empty-state',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section
      class="mt-[18px] rounded-[18px] border border-[color:var(--sr-border)] bg-white/[0.06] p-4 backdrop-blur-[10px]"
      aria-live="polite"
    >
      <p class="mb-2 mt-0 text-base font-extrabold">No hay vuelos para esta búsqueda.</p>
      <p class="m-0 font-semibold text-[color:var(--sr-text-muted)]">
        Probá cambiar fecha, cabina u otro destino.
      </p>
    </section>
  `
})
export class FlightEmptyStateComponent {}
