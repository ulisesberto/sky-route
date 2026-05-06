import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { environment } from '../../../../environments/environment';
import { FlightSearchRequest, FlightResultDto, FlightSearchResponseDto } from '../models/flight-search.models';

@Injectable({ providedIn: 'root' })
export class FlightSearchService {
  private readonly http = inject(HttpClient);

  search(payload: FlightSearchRequest): Observable<FlightResultDto[]> {
    const base = environment.apiBaseUrl.trim().replace(/\/+$/, '');
    const url = base ? `${base}/api/flights/search` : '/api/flights/search';
    return this.http.post<FlightSearchResponseDto>(url, payload).pipe(
      map(body => body.results ?? []),
      catchError((err: unknown) => throwError(() => new Error(this.toUserError(err))))
    );
  }

  private toUserError(err: unknown): string {
    if (err instanceof HttpErrorResponse) {
      if (err.status === 0) return 'No pudimos conectar con el backend. Verificá que esté corriendo.';
      if (err.status >= 500) return 'Ocurrió un error en el servidor. Intentá de nuevo.';
      if (err.status === 400) return 'Revisá los datos ingresados e intentá de nuevo.';
      return 'Ocurrió un error. Intentá de nuevo.';
    }
    return 'Ocurrió un error inesperado. Intentá de nuevo.';
  }
}
