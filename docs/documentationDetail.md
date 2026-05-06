# SkyRoute — Detailed Project Documentation

This document explains how the project works end-to-end (frontend + backend), including the internal layers and the main runtime flows.

## 1) What the system does

SkyRoute is a small travel-aggregator slice that supports:

- Searching flights across multiple mocked providers
- Showing results in the UI with client-side sorting
- Selecting a flight and completing a booking
- Persisting bookings to SQL Server via EF Core

## 2) Tech stack

- **Frontend**: Angular (standalone components, OnPush), Tailwind-based styling
- **Backend**: .NET Web API (`net10.0`)
- **Persistence**: EF Core + SQL Server (LocalDB by default)
- **Tests**:
  - Backend: xUnit (`skyroute-backend-tests/`)
  - Frontend: Angular test runner (Vitest under the hood)

## 3) Repository layout

- `README.md`: how to run the project
- `docs/`: challenge PDF + compliance report + docs
- `skyroute-frontend/`: Angular app
- `skyroute-backend/`: .NET API + business logic + providers + persistence
- `skyroute-backend-tests/`: xUnit test project

## 4) Runtime: how you run it locally

See `README.md`. Key defaults:

- Frontend: `http://localhost:4800`
- Backend: `http://localhost:5000`
- Backend CORS: allows origin `http://localhost:4800`

## 5) Frontend (Angular) — architecture and flow

### 5.1 Key page: `FlightSearchPage`

Location:

- `skyroute-frontend/src/app/pages/flight-search-page/flight-search-page.ts`
- `skyroute-frontend/src/app/pages/flight-search-page/flight-search-page.html`

Responsibilities:

- Holds the top-level UI state as **signals**:
  - `isLoading`, `searchResults`, `hasCompletedSearch`, `errorMessage`
  - `selectedFlight`, `passengers`, `isBookingConfirmed`
- Orchestrates components:
  - Search form
  - Results table
  - Empty state
  - Booking panel

Search submission flow:

1. `FlightSearchFormComponent` emits `(searched)` with a `FlightSearchRequest`.
2. `FlightSearchPage.onSearch(payload)`:
   - sets loading state
   - resets selection + booking state
   - calls `FlightSearchService.search(payload)`
3. On success:
   - `hasCompletedSearch = true`
   - `searchResults = results`
4. On error:
   - `errorMessage = <friendly message>`

### 5.2 Search form component

Location:

- `skyroute-frontend/src/app/pages/flight-search-page/components/flight-search-form/flight-search-form.component.ts`

What it does:

- Reactive form with:
  - origin/destination dropdowns
  - departure date
  - passengers (1–9)
  - cabin class (Economy/Business/First)
- Disables the form while `isLoading` is true
- Validates “origin != destination” via `hasSameRoute`

### 5.3 Results table + client-side sorting

Location:

- `skyroute-frontend/src/app/pages/flight-search-page/components/flight-results-table/flight-results-table.component.ts`

Sorting:

- Sorting happens purely in the component (no extra HTTP call):
  - by `totalPrice`
  - by `durationMinutes`
  - by `departureTime`

Pricing display:

- Shows **Total** (primary) + **Per person** (secondary) for each result row.

### 5.4 Booking panel and passenger validation

Booking orchestration:

- `skyroute-frontend/src/app/pages/flight-search-page/components/booking-panel/booking-panel.component.ts`

Subcomponents:

- Flight summary: `.../flight-summary/flight-summary.component.ts`
- Price breakdown: `.../price-breakdown/price-breakdown.component.ts`
- Passenger form: `.../passenger-form/passenger-form.component.ts`
- Confirmation: `.../booking-confirmation/booking-confirmation.component.ts`

Booking flow in the UI:

1. User clicks “Reservar” on a result row.
2. Booking panel renders:
   - flight summary
   - price breakdown
   - passenger form
3. Passenger form:
   - `Full name`, `Email`, `Document number`
   - Label + validation change based on `flight.isInternational`:
     - International: “Passport Number”, regex `^[A-Z0-9]{6,9}$`
     - Domestic: “National ID”, regex `^[0-9]{6,10}$`
4. On submit, booking panel builds a `BookingRequest` payload and calls `BookingService.confirm(...)`.
5. On success, booking reference is displayed and “bookingConfirmed” is emitted to prevent re-booking the same row.

### 5.5 Frontend API clients

- Flight search client: `skyroute-frontend/src/app/pages/flight-search-page/services/flight-search.service.ts`
  - Calls `POST /api/flights/search`
  - Normalizes errors to user-friendly messages
- Booking client: `skyroute-frontend/src/app/pages/flight-search-page/services/booking.service.ts`
  - Calls `POST /api/bookings`

Both services build the API base URL from `environment.apiBaseUrl` (or fallback to relative `/api/...`).

## 6) Backend (.NET) — architecture and flow

### 6.1 Startup and configuration (`Program.cs`)

Location:

- `skyroute-backend/Program.cs`

Key steps:

- Adds EF Core SQL Server `SkyRouteDbContext`
  - Connection string name: `SkyRouteSql`
- Adds controllers with `JsonStringEnumConverter`
  - This serializes/deserializes enums like `CabinClass` as strings (`"Economy"`, etc.)
- Configures CORS to allow `http://localhost:4800`
- Applies migrations on startup:
  - `db.Database.Migrate()`

### 6.2 Dependency Injection

Location:

- `skyroute-backend/Configuration/DependencyInjection.cs`

Registrations:

- Providers:
  - `IFlightProvider` → `GlobalAirProvider`, `BudgetWingsProvider`
  - Resolved as `IEnumerable<IFlightProvider>`
- Pricing rules:
  - `IPricingRule` → `GlobalAirPricingRule`, `BudgetWingsPricingRule`
  - Resolved as `IEnumerable<IPricingRule>` then matched by `ProviderName`
- Services:
  - `IFlightSearchService` → `FlightSearchService`
  - `IBookingService` → `BookingService`
- Repository:
  - `IBookingRepository` → `BookingRepository`
- Business logic helpers:
  - `IBookingReferenceBusinessLogic` → `BookingReferenceBusinessLogic`
  - `IDocumentValidationBusinessLogic` → `DocumentValidationBusinessLogic`

### 6.3 Controllers (API surface)

#### 6.3.1 Search endpoint: `POST /api/flights/search`

Location:

- `skyroute-backend/Controllers/FlightsController.cs`

Flow:

1. ASP.NET model binding populates `FlightSearchRequestDto`.
2. DataAnnotations + `IValidatableObject` run:
   - Required fields
   - `Passengers` in 1–9
   - `Origin != Destination`
   - `DepartureDate` not in the past (uses `TimeProvider` from DI)
3. If invalid → `ValidationProblem(ModelState)` (HTTP 400)
4. If valid → calls `IFlightSearchService.SearchAsync(...)` and returns `200 OK`

#### 6.3.2 Booking endpoint: `POST /api/bookings`

Location:

- `skyroute-backend/Controllers/BookingsController.cs`

Flow:

1. Model binding populates `CreateBookingRequestDto`.
2. DataAnnotations validate:
   - passengers range
   - passenger required fields + email format
   - flight snapshot required fields
3. If invalid → 400 `ValidationProblem`
4. If valid → `IBookingService.CreateAsync(...)`:
   - document validation (passport vs national id)
   - creates Booking entity + reference code
   - saves via repository
5. Returns `201 Created` with `CreateBookingResponseDto`.

Error handling:

- Invalid document rule → mapped to 400 (ValidationProblem)
- DB save failure (DbUpdateException) → 500 (Problem)

### 6.4 Flight aggregation: providers → offers → results

#### 6.4.1 Provider layer (mocked)

Providers return normalized “offers” (`FlightOfferDto`) based on request parameters.

Common base:

- `skyroute-backend/Providers/ScheduledFlightProvider.cs`

It takes a request and generates offers from a fixed schedule table:

- departure date comes from request
- departure time is built at UTC offset 0
- arrival time computed by adding `DurationMinutes`

Concrete providers:

- `skyroute-backend/Providers/GlobalAirProvider.cs`
- `skyroute-backend/Providers/BudgetWingsProvider.cs`

#### 6.4.2 Pricing rules

Rules implement:

- `IPricingRule.Calculate(decimal baseFare): decimal`

Implementations:

- GlobalAir: `baseFare * 1.15` rounded to 2 decimals
- BudgetWings: `max(round(baseFare * 0.90, 2), 29.99)`

#### 6.4.3 Aggregation service (`FlightSearchService`)

Location:

- `skyroute-backend/Services/FlightSearchService.cs`

Algorithm:

1. Query all providers in parallel (`Task.WhenAll`).
2. Compute `isInternational` once using `AirportRegistry`.
3. For each offer:
   - Apply provider pricing rule (if any), else use base fare
   - Apply **cabin multiplier**:
     - `CabinPriceMultiplier` (Economy ×1.0, Business ×1.6, First ×2.5)
   - Compute:
     - `PerPassengerPrice`
     - `TotalPrice = PerPassengerPrice * passengers`
   - Map to `FlightResultDto` and return to controller

### 6.5 Booking persistence (EF Core)

DbContext:

- `skyroute-backend/Persistence/SkyRouteDbContext.cs`

Entity:

- `skyroute-backend/Models/Booking.cs`

Repository:

- `skyroute-backend/Persistence/BookingRepository.cs`

Service:

- `skyroute-backend/Services/BookingService.cs`

What is stored:

- Booking reference (unique)
- flight snapshot fields
- passenger info
- prices (per passenger + total)
- created timestamp (UTC)

## 7) Data validation (frontend vs backend)

- Frontend provides immediate UX feedback (required fields, patterns).
- Backend re-validates:
  - request DTOs via DataAnnotations
  - route invariants (origin != destination)
  - departure date not in the past
  - document rules in booking service (passport vs national id)

## 8) Testing strategy

Backend tests (`skyroute-backend-tests/`) focus on:

- validation rules (DTO validation)
- pricing rules correctness
- search aggregation behavior
- cabin multiplier behavior

Frontend tests are minimal and validate core build/test tooling.

## 9) Notes / limitations

- Provider mocks are fixed schedules (not real external integrations).
- No authentication/authorization layer.
- No full E2E tests included (manual testing is expected for UI flows).

