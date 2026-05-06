import { ChangeDetectionStrategy, Component } from '@angular/core';

@Component({
  selector: 'app-product-selector',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="mb-[18px] mt-2.5 flex gap-2.5" aria-label="Productos">
      <button
        class="cursor-pointer rounded-full border border-[color:var(--sr-border)] bg-[rgba(26,108,255,0.18)] px-4 py-2.5 font-bold text-[color:var(--sr-text)] [border-color:rgba(26,108,255,0.4)]"
        type="button"
      >
        Vuelos
      </button>
      <button
        class="cursor-not-allowed rounded-full border border-[color:var(--sr-border)] bg-transparent px-4 py-2.5 font-bold text-[color:var(--sr-text-muted)] opacity-55"
        type="button"
        disabled
      >
        Hoteles
      </button>
      <button
        class="cursor-not-allowed rounded-full border border-[color:var(--sr-border)] bg-transparent px-4 py-2.5 font-bold text-[color:var(--sr-text-muted)] opacity-55"
        type="button"
        disabled
      >
        Autos
      </button>
    </section>
  `
})
export class ProductSelectorComponent {}
