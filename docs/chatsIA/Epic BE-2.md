# Epic BE-2 — Proceso de desarrollo con Dev + QA

**Fecha:** Miércoles 6 de mayo de 2026  
**Branch:** `epic_be-2`  
**Agente activo:** John (PM), coordinando a Amelia (Dev) y QA

---

## Prompt del usuario

> @pm encaremos la EPIC BE-2 habla con el dev y qa para el desarrollo, cuando el dev termine una US, llama al QA para revisar ese desarrollo, si hay bugs cargalos al archivo epics and stories vuelve a llamar al dev para resolver hasta que no haya bugs, luego pasar a la siguiente US y repetir el proceso

---

## Contexto inicial relevado por el PM

- Archivo de planificación: `_bmad-output/planning-artifacts/epics-and-stories.md`
- Backend: `skyroute-backend` (.NET)
- Tests: `skyroute-backend-tests/SkyRoute.Api.Tests`
- Configuración del proyecto: `user_name: ulises`, `communication_language: spanish`
- **Estado inicial de EPIC BE-2:** marcada como `⬜ Pendiente` en el doc, pero el código ya existía parcialmente en el repo.
- Tests existentes antes de iniciar: **32 pasando**.

### US de EPIC BE-2

| US | Descripción | Estado inicial |
|----|-------------|---------------|
| BE-2.1 | Mock GlobalAir | ⬜ Pendiente |
| BE-2.2 | Mock BudgetWings | ⬜ Pendiente |
| BE-2.3 | Abstracción para agregar providers | ⬜ Pendiente |

---

## US BE-2.1 — Mock GlobalAir

### 👩‍💻 Amelia (Dev)

Verificó que `GlobalAirProvider.cs` ya existía y cumplía todos los AC y tareas (T1–T4):
- Clase `sealed GlobalAirProvider : IFlightProvider` en `Providers/`
- 4 vuelos con horarios y BaseFare variados: GA101/GA207/GA318/GA455
- Registrado en `DependencyInjection.cs`

Creó `GlobalAirProviderTests.cs` con 7 tests unitarios cubriendo: retorno ≥3 vuelos, FlightNumber no vacío, DepartureTime según fecha del request, ArrivalTime = Departure + Duration, BaseFare > 0, FlightNumbers únicos, CancellationToken cancelado.

**Resultado: 39/39 tests pasando.**

---

### 🔍 QA — Revisión BE-2.1

**Veredicto: APROBADO CON OBSERVACIONES** — sin críticos, 3 menores accionables.

| Bug | Descripción | Severidad |
|-----|-------------|-----------|
| BUG-BE2-01 | Tests no verificaban `Provider == "GlobalAir"` | 🟡 Menor |
| BUG-BE2-02 | Sin test que confirmara variedad de BaseFare | 🟡 Menor |
| BUG-BE2-03 | `ThrowIfNull(request)` sin cobertura de test | 🟡 Menor |

Bugs registrados en `epics-and-stories.md`.

---

### 👩‍💻 Amelia — Fix BE-2.1

Agregó 3 tests a `GlobalAirProviderTests.cs`:
- `AllFlightsHaveGlobalAirProvider`
- `BasefaresAreVariedAcrossFlights`
- `NullRequest_ThrowsArgumentNullException`

**Resultado: 42/42 tests pasando. Todos los bugs cerrados.**

---

## US BE-2.2 — Mock BudgetWings

### 👩‍💻 Amelia (Dev)

Verificó que `BudgetWingsProvider.cs` ya existía y cumplía T1–T3:
- 4 vuelos diferenciados: BW501/BW612/BW748/BW893 con horarios y precios distintos a GlobalAir
- Registrado en `DependencyInjection.cs`

Creó `BudgetWingsProviderTests.cs` con 10 tests análogos a GlobalAir.

**Resultado: 53/53 tests pasando.**

---

### 🔍 QA — Revisión BE-2.2

**Veredicto: APROBADO CON OBSERVACIONES** — sin críticos, 5 menores accionables.

| Bug | Descripción | Severidad |
|-----|-------------|-----------|
| BUG-BE2-04 | Sin guard clauses para `Origin`/`Destination` nulos en el provider | 🟡 Menor |
| BUG-BE2-05 | Tests no verificaban propagación de `Origin`/`Destination` | 🟡 Menor |
| BUG-BE2-06 | Aserción de conteo laxa (`>= 3` en lugar de `== 4`) | 🟡 Menor |
| BUG-BE2-07 | Sin test de variedad de `DurationMinutes` | 🟡 Menor |
| BUG-BE2-08 | Sin verificación de offset UTC en `DepartureTime` | 🟡 Menor |

Bugs registrados en `epics-and-stories.md`.

---

### 👩‍💻 Amelia — Fix BE-2.2

- Agregó `ArgumentException.ThrowIfNullOrWhiteSpace` para `Origin`/`Destination` en `BudgetWingsProvider.cs` **y también en `GlobalAirProvider.cs`** por consistencia.
- Cambió `Assert.True(result.Count >= 3)` → `Assert.Equal(4, result.Count)` en ambos archivos de test.
- Agregó tests: `OriginAndDestinationMatchRequest`, `DurationMinutesAreVaried`, verificación de `TimeSpan.Zero` en offset UTC.

**Resultado: 55/55 tests pasando. Todos los bugs cerrados.**

---

## US BE-2.3 — Abstracción para agregar providers

### 👩‍💻 Amelia (Dev)

Verificó que la arquitectura ya cumplía los 3 AC:
- `IFlightProvider` definida en `Interfaces/IBusinessLogic/`
- `FlightOfferDto` como modelo interno separado de `FlightResultDto`
- Providers registrados como `IEnumerable<IFlightProvider>` en DI → `FlightSearchService` los itera con `_providers.Select(...)` sin hardcodear tipos

Agregó 2 tests nuevos a `FlightSearchServiceTests.cs`:
- `SearchAsync_ThirdProviderAddedViaDI_IsQueriedAutomatically` — demuestra extensibilidad sin tocar código existente
- `SearchAsync_OneProviderThrows_ExceptionPropagates` — documenta comportamiento actual de `Task.WhenAll`

**Resultado: 57/57 tests pasando.**

---

### 🔍 QA — Revisión BE-2.3

**Veredicto: APROBADO CON OBSERVACIONES** — 1 crítico + 2 menores.

| Bug | Descripción | Severidad |
|-----|-------------|-----------|
| BUG-BE2-09 | `GlobalAirProvider` y `BudgetWingsProvider` tenían lógica de construcción 100% duplicada — agregar un tercer provider requería copiar todo el boilerplate | 🔴 Crítico |
| BUG-BE2-10 | `batches.SelectMany(offers => offers)` lanzaría `NullReferenceException` si un provider retorna `null` | 🟡 Menor |
| BUG-BE2-11 | Helper `MakeProvider` en tests tenía parámetro `providerName` sin usar | 🟡 Menor |

Bugs registrados en `epics-and-stories.md`.

---

### 👩‍💻 Amelia — Fix BE-2.3

**BUG-BE2-09:** Creó `Providers/ScheduledFlightProvider.cs` — clase base abstracta con el loop de construcción de vuelos. `GlobalAirProvider` y `BudgetWingsProvider` simplificados a solo sus propiedades `ProviderName` y `Schedule`. Agregar un nuevo provider ahora requiere solo 2 propiedades + 1 línea en DI.

**BUG-BE2-10:** En `FlightSearchService.cs` cambió `SelectMany(offers => offers)` → `SelectMany(offers => offers ?? [])`.

**BUG-BE2-11:** Eliminó el parámetro `string providerName` del helper `MakeProvider` y actualizó los 11 call sites.

**Resultado: 57/57 tests pasando. Todos los bugs cerrados.**

---

## Resultado final

| US | Estado | Tests agregados |
|----|--------|-----------------|
| BE-2.1 | ✅ Completado | 10 (7 iniciales + 3 fixes QA) |
| BE-2.2 | ✅ Completado | 13 (10 iniciales + 3 fixes QA) |
| BE-2.3 | ✅ Completado | 2 de extensibilidad |
| **EPIC BE-2** | ✅ Completado | **57 tests totales** (25 nuevos en esta sesión) |

### Artefactos modificados

| Archivo | Cambio |
|---------|--------|
| `skyroute-backend/Providers/ScheduledFlightProvider.cs` | Nuevo — clase base abstracta |
| `skyroute-backend/Providers/GlobalAirProvider.cs` | Refactorizado para heredar de `ScheduledFlightProvider` + guard clauses |
| `skyroute-backend/Providers/BudgetWingsProvider.cs` | Refactorizado para heredar de `ScheduledFlightProvider` + guard clauses |
| `skyroute-backend/Services/FlightSearchService.cs` | Null guard en `SelectMany` |
| `skyroute-backend-tests/.../GlobalAirProviderTests.cs` | Nuevo — 10 tests unitarios |
| `skyroute-backend-tests/.../BudgetWingsProviderTests.cs` | Nuevo — 13 tests unitarios |
| `skyroute-backend-tests/.../FlightSearchServiceTests.cs` | 2 tests nuevos + cleanup del helper |
| `_bmad-output/planning-artifacts/epics-and-stories.md` | EPIC BE-2 y sus 3 US marcadas ✅, bugs documentados y cerrados |
