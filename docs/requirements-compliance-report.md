# SkyRoute Developer Challenge — Informe de Cumplimiento

Fuente de requisitos: `docs/SkyRoute_Developer_Challenge_CANDIDATE 1 (1).pdf` (4 páginas).

Fecha del informe: 2026-05-06

## Resumen ejecutivo

El desarrollo **cumple** los requisitos funcionales principales de **Flight Search**, **Sorting**, **Booking Flow** y **Backend API** (Angular + .NET), incluyendo reglas de pricing por provider y validación dinámica de documento (doméstico vs internacional).

## Evidencia técnica (runs / tests)

- **Backend**: .NET SDK `10.0.202`
  - Tests: `dotnet test skyroute-backend-tests/SkyRoute.Api.Tests/SkyRoute.Api.Tests.csproj` → **84 passed**
- **Frontend**: Node `v22.22.0`, npm `10.9.2`
  - Build: `npx ng build --configuration=development` → OK (dist generado)
  - Tests: `npm test` (Vitest) → **2 passed**

## Cumplimiento por sección del PDF

### 2. Business Context — Providers + Pricing Rules

- **Integración con 2 providers (mock en backend)**: ✅
  - `skyroute-backend/Providers/GlobalAirProvider.cs`
  - `skyroute-backend/Providers/BudgetWingsProvider.cs`
  - Ambos providers retornan resultados “realistas” (schedule fijo con múltiples vuelos) para cualquier búsqueda.

- **Reglas de pricing por provider**: ✅
  - GlobalAir: “Base fare + 15% fuel surcharge, round to 2 decimals” → ✅
    - `skyroute-backend/BusinessLogic/GlobalAirPricingRule.cs`
  - BudgetWings: “Base fare − 10%, min $29.99, discount on base fare only” → ✅
    - `skyroute-backend/BusinessLogic/BudgetWingsPricingRule.cs`

- **Extensibilidad para futuros providers**: ✅ (DI + agregación)
  - `skyroute-backend/Interfaces/IBusinessLogic/IPricingRule.cs`
  - `skyroute-backend/Services/FlightSearchService.cs` (itera `IEnumerable<IFlightProvider>`, aplica regla por provider)

### 3.1 Flight Search (Frontend + API)

- **Formulario captura**: ✅
  - Origen / Destino: ✅ (dropdown, hardcoded ≥6 aeropuertos, ≥2 países)
    - `skyroute-frontend/src/app/pages/flight-search-page/flight-search-page.ts` (`airports` con 6 entradas, AR/CL/PE/US)
  - Departure date: ✅
    - `flight-search-form.component.ts` (`departureDate`, required)
  - Passengers (1–9): ✅
    - Front: validators min/max + input min/max
    - Back: `[Range(1,9)]` en `FlightSearchRequestDto.Passengers`
  - Cabin class Economy/Business/First: ✅
    - Front: select (`cabinClasses`)
    - Back: `CabinClass` enum + `JsonStringEnumConverter`

- **Frontend muestra resultados con campos requeridos**: ✅
  - Provider, Flight number, departure, arrival, duration, cabin class, price
  - `flight-results-table.component.ts`

- **Pricing display: Total vs Per-person, con distinción clara**: ✅
  - UI: total en negrita + “per person” secundario
  - `flight-results-table.component.ts`

### 3.2 Flight Results & Sorting

- **Sorting por Price, Duration, Departure time**: ✅
  - `flight-results-table.component.ts` (sort local)
- **Sorting en frontend (sin nuevos calls)**: ✅
- **Loading indicator**: ✅
  - Botón “Buscando…” + spinner en el formulario
  - `flight-search-form.component.ts`
- **Empty state**: ✅
  - `flight-search-page.html` + `FlightEmptyStateComponent`

### 3.3 Booking Flow

Cuando el usuario selecciona un vuelo, el flujo requerido está cubierto:

- **Resumen del vuelo seleccionado**: ✅
- **Price breakdown (per pax, passengers, total)**: ✅
- **Passenger form (full name, email, document number)**: ✅
- **Confirm booking → retorna booking reference**: ✅
  - Front booking panel: `skyroute-frontend/src/app/pages/flight-search-page/components/booking-panel/`
  - Backend endpoint: `POST /api/bookings` en `skyroute-backend/Controllers/BookingsController.cs`

**Document Type Requirement (International vs Domestic)**: ✅
- Label: “Passport Number” vs “National ID” → ✅
- Validación cambia según ruta seleccionada → ✅
  - Front: `PassengerFormComponent` usa `isInternational` para label + regex y mensaje.
    - `passenger-form.component.ts` (`documentLabel`, `updateDocumentValidators`)
  - Backend: `DocumentValidationBusinessLogic` (reglas de validación de documento) y mapeo a 400.
    - `skyroute-backend/BusinessLogic/DocumentValidationBusinessLogic.cs`
    - `BookingsController` captura `DocumentRuleViolationException` → `ValidationProblem` (400)

### 3.4 Backend API

- **Endpoints implementados**: ✅
  - Buscar vuelos: `POST /api/flights/search`
    - `skyroute-backend/Controllers/FlightsController.cs`
  - Confirmar booking: `POST /api/bookings`
    - `skyroute-backend/Controllers/BookingsController.cs`

- **Contratos de request/response**: ✅
  - `skyroute-backend/DTOs/FlightSearchRequestDto.cs`
  - `skyroute-backend/DTOs/FlightSearchResponseDto.cs`
  - `skyroute-backend/DTOs/FlightResultDto.cs`
  - Booking DTOs presentes en `skyroute-backend/DTOs/` (ver controller)

### 4. Technology (Angular + .NET)

- **Angular**: ✅ (`skyroute-frontend/`)
- **.NET**: ✅ (`skyroute-backend/`, target `net10.0`)
  - `skyroute-backend/SkyRoute.Api.csproj` (TargetFramework net10.0)

### 5. Deliverables

1) **Aplicación corre localmente (frontend + backend)**: ✅ (estructura y build/test OK; backend usa LocalDB por `appsettings.json`)
   - Backend URL (launchSettings): `http://localhost:5000`
   - CORS para frontend: `http://localhost:4800`

2) **Source code en repo**: ✅

3) **README con setup/run + decisiones de arquitectura + trade-offs/limitaciones**: ❌ / Parcial
   - Encontrado: `skyroute-frontend/README.md` (boilerplate Angular CLI)
   - No encontrado: `README.md` en raíz o documentación equivalente que cubra **backend + frontend** y decisiones.


