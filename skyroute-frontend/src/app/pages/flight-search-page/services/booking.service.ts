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
      if (err.status === 0) return 'No pudimos conectar con el backend. Verificá que esté corriendo.';
      if (err.status >= 500) return 'Error en el servidor al confirmar la reserva. Intentá de nuevo.';
      if (err.status === 400) return 'Datos de reserva inválidos. Revisá el formulario e intentá de nuevo.';
      return 'No se pudo confirmar la reserva. Intentá de nuevo.';
    }
    return 'Error inesperado al confirmar la reserva.';
  }
}
