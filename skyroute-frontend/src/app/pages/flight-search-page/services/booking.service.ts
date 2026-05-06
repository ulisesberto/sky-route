import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { BookingRequest, BookingResponse } from '../models/flight-search.models';

@Injectable({ providedIn: 'root' })
export class BookingService {
  private readonly http = inject(HttpClient);

  confirm(payload: BookingRequest): Observable<string> {
    const base = environment.apiBaseUrl.trim().replace(/\/+$/, '');
    const url = base ? `${base}/api/bookings` : '/api/bookings';
    return this.http.post<BookingResponse>(url, payload).pipe(
      map(body => body.bookingReference),
      catchError((err: unknown) => throwError(() => new Error(this.toBookingError(err))))
    );
  }

  private toBookingError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 0) return 'We ran into a connection issue while confirming your booking. Please try again.';
      if (err.status >= 500) return 'We could not confirm your booking right now. Please try again.';
      if (err.status === 400) return 'Please check your details and try again.';
      return 'We could not confirm your booking. Please try again.';
    }
    return 'We could not confirm your booking. Please try again.';
  }
}
