export type CabinClass = 'Economy' | 'Business' | 'First';
export type SortField = 'price' | 'duration' | 'departure';
export type SortDir = 'asc' | 'desc';

export interface AirportOption {
  code: string;
  name: string;
  city: string;
  countryCode: string;
}

export interface FlightSearchRequest {
  origin: string;
  destination: string;
  departureDate: string;
  passengers: number;
  cabinClass: CabinClass;
}

export interface FlightResultDto {
  provider: string;
  flightNumber: string;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  durationMinutes: number;
  cabinClass: string;
  perPassengerPrice: number;
  totalPrice: number;
  currency: string;
  isInternational: boolean;
}

export interface FlightSearchResponseDto {
  results: FlightResultDto[];
}

export interface FlightSnapshot {
  provider: string;
  flightNumber: string;
  origin: string;
  destination: string;
  departureTime: string;
  arrivalTime: string;
  cabinClass: string;
  perPassengerPrice: number;
  totalPrice: number;
  currency: string;
  isInternational: boolean;
}

export interface PassengerData {
  fullName: string;
  email: string;
  documentNumber: string;
}

export interface BookingRequest {
  flight: FlightSnapshot;
  passengers: number;
  passenger: PassengerData;
}

export interface BookingResponse {
  bookingReference: string;
}
