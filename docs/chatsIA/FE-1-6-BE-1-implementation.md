# FE-1 & BE-1 — proceso de creación (chat export)

> Export compacto: **prompts del usuario** + **qué hizo el agente** (sin razonamiento interno).  
> Título referido del chat: *FE-! 6 BE-1 implementation* → archivo: `FE-1-6-BE-1-implementation.md` (nombre saneado para filesystem).

---

## 1. Product / PM

**Prompt:** `@pm` leer PDF en `docs`, generar epics y user stories separando frontend y backend.

**Resultado:** Epics y US listadas en la respuesta (Angular vs .NET); luego se pidió persistir en Markdown.

---

## 2. Artefactos en `_bmad-output`

**Prompt:** Generar lo anterior en carpeta `output` como `.md`.

**Resultado:** `_bmad-output/planning-artifacts/epics-and-stories.md`.

---

## 3. Arquitectura

**Prompt:** `@architect` documento de arquitectura según planning-artifacts.

**Resultado:** `_bmad-output/planning-artifacts/architecture-recommended.md` (API, capas, EF sugerido después).

**Prompt (seguido):** Incluir Entity Framework y SQL.

**Resultado:** Misma doc ampliada con `DbContext`, conexión SQL, entidad `Bookings`, migraciones.

---

## 4. UX

**Prompt:** `@ux-designer` diseño frontend usando imágenes en `docs/images`.

**Resultado:** `_bmad-output/planning-artifacts/ux-frontend-design.md` alineado a `design1.png` / `design2.png`.

---

## 5. Frontend Angular — Epic FE-1

**Prompt:** `@dev` con contexto planning-artifacts: crear frontend Angular, solo **EPIC FE-1** y todas sus US; que compile y corra local.

**Resultado:** Proyecto `skyroute-frontend/`: formulario búsqueda (aeropuertos hardcodeados ≥6 en ≥2 países, fecha, pasajeros 1–9, cabina, swap, loading, POST a `/api/flights/search`), estilos tipo mockups. `npm run build` verificado.

---

## 6. Git / repo nuevo

**Prompt:** `.gitignore` raíz para no subir BMad; luego desvincular git viejo y pushear **solo** `skyroute-frontend` + `.gitignore` al repo `sky-route`.

**Resultado:** `.gitignore` raíz (`_bmad`, `_bmad-output`, `.agents`, `docs`); repo reinicializado y push al remoto indicado.

---

## 7. Backend .NET — Epic BE-1

**Prompt:** `@dev` backend .NET + EF según planning-artifacts, **BE-1**; que corra al final.

**Resultado:** `skyroute-backend/`: Web API, DTOs/endpoints `POST /api/flights/search` (lista vacía inicial) y `POST /api/bookings`, EF Core + SQL LocalDB, migración inicial, CORS hacia `4200`, perfil `http` en `5000`.

---

## 8. Arquitectura en capas en backend

**Prompt:** Estructura limpia: `Controllers`, `Models`, `DTOs`, `Services`, `BusinessLogic`, `Interfaces/` con subcarpetas `IServices`, `IBusinessLogic`, `IModels`, `IRepositories`; controladores sin negocio; servicios por interfaces.

**Resultado:** Refactor: capas, DI en `Configuration/DependencyInjection.cs`, repositorio `BookingRepository`, servicios, excepciones de mapeo HTTP, entidad `Booking` en `Models`, migraciones EF ajustadas al nombre de tipo.

---

## 9. Convención de nombres (DTO / BusinessLogic)

**Prompt:** Archivos DTO con sufijo `Dto`; BusinessLogic e interfaces `*BusinessLogic`; modelos sin cambio de criterio general.

**Resultado:** Renombrados `FlightSearchRequestDto`, `FlightSearchResponseDto`, `CreateBookingRequestDto`, `CreateBookingResponseDto`; `IBookingReferenceBusinessLogic` + `BookingReferenceBusinessLogic`; referencias actualizadas en servicios, controladores y DI.

---

## 10. Backend `.gitignore`

**Prompt:** `.gitignore` en `skyroute-backend` solo build .NET.

**Resultado:** `bin/`, `obj/`.

**Prompt (seguido):** Ignorar `appsettings.Development.json`.

**Resultado:** Añadido a `skyroute-backend/.gitignore`.

---

## 11. Appsettings / EF CLI

**Prompt (vía error en terminal):** Por qué `dotnet ef database update` dice que no encuentra `SkyRouteSql`; si ignora `Development`.

**Resultado (agente):** Causa real: clave `SkyRouteSql` en código vs `DefaultConnection` en JSON; se unificó a `SkyRouteSql` en `appsettings` y se explicó layering `appsettings` + `appsettings.Development.json` y entorno en design-time.

---

## 12. CORS

**Prompt:** Error CORS: origen `http://localhost:61513` vs API en `5000`.

**Resultado:** En `Development`, política CORS con `SetIsOriginAllowed` para `localhost`/`127.0.0.1` (cualquier puerto); producción con orígenes cerrados.

---

## 13. Preguntas en modo Ask (sin cambios de código)

- Cómo correr el backend (`dotnet run`, LocalDB, URLs).
- Qué probar (`POST` search y bookings con bodies de ejemplo).
- Comando migraciones: `dotnet ef database update` vs `Update-Database` en VS.
- Qué es `DependencyInjection.cs` y por qué no está todo en `Program.cs` (organización y extensión).

---

## Estado final útil del workspace (resumen)

| Área | Ubicación principal |
|------|---------------------|
| Frontend FE-1 | `skyroute-frontend/` |
| Backend BE-1 + capas | `skyroute-backend/` |
| Epics / arquitectura / UX | `_bmad-output/planning-artifacts/` (repo puede ignorar `_bmad-output` según `.gitignore` raíz) |

---

*Generado como resumen de sesión; detalle fino de código en los archivos citados.*
