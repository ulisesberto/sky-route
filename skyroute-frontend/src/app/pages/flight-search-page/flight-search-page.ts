import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { finalize } from 'rxjs';
import { AirportOption, CabinClass, FlightResultDto, FlightSearchRequest } from './models/flight-search.models';
import { FlightSearchService } from './services/flight-search.service';
import { ProductSelectorComponent } from './components/product-selector/product-selector.component';
import { FlightSearchFormComponent } from './components/flight-search-form/flight-search-form.component';
import { FlightResultsTableComponent } from './components/flight-results-table/flight-results-table.component';
import { FlightEmptyStateComponent } from './components/flight-empty-state/flight-empty-state.component';
import { BookingPanelComponent } from './components/booking-panel/booking-panel.component';

@Component({
  selector: 'app-flight-search-page',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [
    ProductSelectorComponent,
    FlightSearchFormComponent,
    FlightResultsTableComponent,
    FlightEmptyStateComponent,
    BookingPanelComponent
  ],
  templateUrl: './flight-search-page.html',
  styleUrl: './flight-search-page.scss'
})
export class FlightSearchPage {
  private readonly flightSearchService = inject(FlightSearchService);

  readonly airports: AirportOption[] = [
    { code: 'EZE', name: 'Buenos Aires (EZE)', city: 'Buenos Aires', countryCode: 'AR' },
    { code: 'AEP', name: 'Buenos Aires (AEP)', city: 'Buenos Aires', countryCode: 'AR' },
    { code: 'SCL', name: 'Santiago (SCL)', city: 'Santiago', countryCode: 'CL' },
    { code: 'LIM', name: 'Lima (LIM)', city: 'Lima', countryCode: 'PE' },
    { code: 'MIA', name: 'Miami (MIA)', city: 'Miami', countryCode: 'US' },
    { code: 'JFK', name: 'New York (JFK)', city: 'New York', countryCode: 'US' }
  ];

  readonly cabinClasses: CabinClass[] = ['Economy', 'Business', 'First'];

  readonly isLoading = signal(false);
  readonly searchResults = signal<FlightResultDto[]>([]);
  readonly hasCompletedSearch = signal(false);
  readonly errorMessage = signal<string | null>(null);
  readonly selectedFlight = signal<FlightResultDto | null>(null);
  readonly passengers = signal<number>(1);

  readonly showResultsTable = computed(() => this.hasCompletedSearch() && this.searchResults().length > 0);
  readonly showEmptyState = computed(() => this.hasCompletedSearch() && this.searchResults().length === 0);

  onSearch(payload: FlightSearchRequest): void {
    this.isLoading.set(true);
    this.searchResults.set([]);
    this.hasCompletedSearch.set(false);
    this.errorMessage.set(null);
    this.selectedFlight.set(null);
    this.passengers.set(payload.passengers);

    this.flightSearchService.search(payload).pipe(
      finalize(() => this.isLoading.set(false))
    ).subscribe({
      next: (results) => {
        this.hasCompletedSearch.set(true);
        this.searchResults.set(results);
      },
      error: (err: Error) => {
        this.errorMessage.set(err.message);
      }
    });
  }

  onFlightSelected(flight: FlightResultDto): void {
    this.selectedFlight.set(flight);
  }
}
