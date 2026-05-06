import { ChangeDetectionStrategy, Component, inject, Input, OnChanges, SimpleChanges } from '@angular/core';
import { finalize } from 'rxjs';
import { BookingRequest, FlightResultDto, PassengerData } from '../../models/flight-search.models';
import { BookingService } from '../../services/booking.service';
import { FlightSummaryComponent } from './flight-summary/flight-summary.component';
import { PriceBreakdownComponent } from './price-breakdown/price-breakdown.component';
import { PassengerFormComponent } from './passenger-form/passenger-form.component';
import { BookingConfirmationComponent } from './booking-confirmation/booking-confirmation.component';

@Component({
  selector: 'app-booking-panel',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [FlightSummaryComponent, PriceBreakdownComponent, PassengerFormComponent, BookingConfirmationComponent],
  template: `
    <section
      class="mt-[18px] rounded-[18px] border border-[color:var(--sr-border)] bg-white/[0.06] p-5 backdrop-blur-[10px]"
      aria-label="Panel de reserva"
    >
      <h2 class="mb-4 mt-0 text-lg font-extrabold">Reservar vuelo</h2>
      <app-flight-summary [flight]="flight" />
      <app-price-breakdown [flight]="flight" [passengers]="passengers" />
      @if (bookingReference) {
        <app-booking-confirmation [bookingReference]="bookingReference" />
      }
      @if (!bookingReference) {
        <app-passenger-form
          [isInternational]="flight.isInternational"
          [isLoading]="isLoading"
          [errorMessage]="errorMessage"
          (formSubmitted)="onFormSubmitted($event)"
          (retried)="onRetried()"
        />
      }
    </section>
  `
})
export class BookingPanelComponent implements OnChanges {
  private readonly bookingService = inject(BookingService);

  @Input({ required: true }) flight!: FlightResultDto;
  @Input({ required: true }) passengers!: number;

  isLoading = false;
  bookingReference: string | null = null;
  errorMessage: string | null = null;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['flight']) {
      this.bookingReference = null;
      this.errorMessage = null;
      this.isLoading = false;
    }
  }

  onFormSubmitted(passenger: PassengerData): void {
    const payload: BookingRequest = {
      flight: {
        provider: this.flight.provider,
        flightNumber: this.flight.flightNumber,
        origin: this.flight.origin,
        destination: this.flight.destination,
        departureTime: this.flight.departureTime,
        arrivalTime: this.flight.arrivalTime,
        cabinClass: this.flight.cabinClass,
        perPassengerPrice: this.flight.perPassengerPrice,
        totalPrice: this.flight.totalPrice,
        currency: this.flight.currency,
        isInternational: this.flight.isInternational
      },
      passengers: this.passengers,
      passenger
    };

    this.isLoading = true;
    this.errorMessage = null;

    this.bookingService.confirm(payload).pipe(
      finalize(() => (this.isLoading = false))
    ).subscribe({
      next: (ref) => { this.bookingReference = ref; },
      error: (err: Error) => { this.errorMessage = err.message; }
    });
  }

  onRetried(): void {
    this.errorMessage = null;
  }
}
