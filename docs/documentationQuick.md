# SkyRoute — Quick Project Docs

## What this is

Small “Flight Search & Booking” slice of SkyRoute (travel aggregator) built with **Angular** (frontend) + **.NET** (backend).

## High-level architecture

- **Frontend (Angular)**: search form → results table (client-side sorting) → booking panel
- **Backend (.NET API)**: controllers → services → providers/pricing rules → EF Core persistence

## Repo structure

- `skyroute-frontend/`: Angular app
- `skyroute-backend/`: .NET Web API + EF Core
- `skyroute-backend-tests/`: xUnit tests (API/service/business logic)
- `docs/`: challenge PDF + compliance report + this doc

## Main user flows

### 1) Flight search

1. User fills: origin, destination, date, passengers (1–9), cabin class.
2. Frontend calls backend: `POST /api/flights/search`.
3. Frontend displays results table with:
   - provider, flight number, departure/arrival, duration, cabin class
   - **total price** (primary) and **per-person price** (secondary)
4. Sorting is **frontend-only** (price, duration, departure time).

### 2) Booking

1. User selects a flight from results.
2. Booking panel shows summary + price breakdown.
3. Passenger form captures: full name, email, document number.
4. Document label + validation change based on route:
   - International: **Passport Number**
   - Domestic: **National ID**
5. Frontend calls backend: `POST /api/bookings` and shows a booking reference code on success.

## Backend API

- `POST /api/flights/search`
  - Request: `FlightSearchRequestDto`
  - Response: `FlightSearchResponseDto` (list of `FlightResultDto`)
- `POST /api/bookings`
  - Request: `CreateBookingRequestDto`
  - Response: `CreateBookingResponseDto` (booking reference)

Ports:
- Frontend: `http://localhost:4800`
- Backend: `http://localhost:5000`

## Providers + pricing

Backend aggregates flight offers from **mock providers**:

- `GlobalAir`
- `BudgetWings`

Pricing rules (applied per provider):

- **GlobalAir**: base fare + 15% (rounded to 2 decimals, AwayFromZero)
- **BudgetWings**: base fare − 10% (rounded), minimum price 29.99

Cabin price scaling:

- Economy × 1.0
- Business × 1.6
- First × 2.5

Order of operations:
1) provider pricing rule → 2) cabin multiplier → 3) total = perPassenger × passengers

## Persistence

- EF Core (SQL Server)
- Table: `Bookings` (booking reference + passenger data + flight snapshot fields)
- Migrations auto-applied on backend startup (`Database.Migrate()`).

Default connection string (LocalDB) lives in `skyroute-backend/appsettings.json`.

## Testing

- Backend: xUnit tests in `skyroute-backend-tests/`
- Frontend: Angular/Vitest tests via `npm test`

## Known limitations (by design / scope)

- Providers are mocked (fixed schedules, no real airline APIs).
- No authentication/roles/users (no “admin/superuser”).
- No dedicated end-to-end browser tests (manual smoke is supported).

