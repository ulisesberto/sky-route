# Definicion de US y Bugs: estabilización

Fecha: 2026-05-06

Objetivo del chat: documentar el proceso compacto de creación de una User Story de estabilización para el proyecto SkyRoute, a partir de un error detectado al confirmar una reserva desde el formulario de pasajero.

Nota sobre el archivo: Windows no permite `:` en nombres de archivo, por eso el archivo se guardó como `Definicion de US y Bugs - estabilización.md`.

---

## 1. Prompt del usuario: organizar una nueva US para estabilizar el proyecto

El usuario pidió al PM organizar una nueva User Story con tareas para estabilizar el proyecto. Reportó un error al llenar el formulario del pasajero (`name`, `email`, `passportnumber`) y hacer clic en `Confirmar reserva`.

El error principal reportado fue:

```text
System.ArgumentException: 999999.99 is not a valid value for Decimal. (Parameter 'value')
 ---> System.FormatException: The input string '999999.99' was not in a correct format.
```

El stack trace indicaba que el fallo ocurría durante la validación de DataAnnotations, específicamente en:

```text
System.ComponentModel.DataAnnotations.RangeAttribute.SetupConversion()
```

## 2. Resultado del agente

El agente activó el rol de PM (`John`) y exploró el proyecto para entender la estructura, los artefactos existentes y el flujo afectado.

Resumen de lo encontrado:

- El proyecto tiene backend en ASP.NET Core y frontend en Angular.
- El flujo afectado es `POST /api/bookings`.
- El frontend envía un `BookingRequest` con datos del vuelo seleccionado, cantidad de pasajeros y datos del pasajero.
- El backend recibe `CreateBookingRequestDto`, que contiene un `FlightSnapshotDto`.
- El crash ocurre antes de llegar al servicio de booking, durante el model binding y la validación de DataAnnotations.

La causa raíz identificada fue:

```csharp
[Range(typeof(decimal), "0", "999999.99")]
public decimal PerPassengerPrice { get; set; }

[Range(typeof(decimal), "0", "9999999.99")]
public decimal TotalPrice { get; set; }
```

Estos atributos están en `skyroute-backend/DTOs/FlightSnapshotDto.cs`.

El problema es que la sobrecarga de strings de `[Range]` parsea los límites usando la cultura actual del servidor. En culturas hispanoamericanas, donde el separador decimal suele ser coma, el string `"999999.99"` con punto no se puede convertir a `decimal`.

## 3. US creada por el agente

El agente creó una User Story detallada en:

```text
_bmad-output/implementation-artifacts/1-1-fix-decimal-range-culture-crash.md
```

Título:

```text
Story 1.1: Corregir crash de validación decimal por cultura en BookingsController
```

Historia:

```text
Como usuario registrado,
quiero poder completar el formulario de pasajero (nombre, email, número de pasaporte) y confirmar una reserva,
para que el sistema procese mi reserva sin errores de servidor al validar los precios del vuelo.
```

Criterios de aceptación definidos:

- `POST /api/bookings` con body válido retorna HTTP 201 independientemente de la cultura del servidor.
- La validación de rango sigue funcionando: precios negativos o superiores al máximo retornan HTTP 400.
- No se introducen cambios en el schema de base de datos ni migraciones EF Core.
- Los tests existentes siguen pasando.
- Se agrega al menos un test que verifica que un precio válido con punto decimal (`12345.67`) no provoca error 500.

Tareas principales definidas:

- Corregir los `[Range]` de `FlightSnapshotDto.cs`.
- Verificar que no haya otros `[Range(typeof(decimal), ...)]` afectados.
- Validar casos de frontera.
- Agregar tests de booking.
- Ejecutar smoke test end-to-end desde el frontend.

Solución técnica recomendada:

```csharp
[Range(typeof(decimal), "0", "999999.99", ParseLimitsInInvariantCulture = true)]
public decimal PerPassengerPrice { get; set; }

[Range(typeof(decimal), "0", "9999999.99", ParseLimitsInInvariantCulture = true)]
public decimal TotalPrice { get; set; }
```

## 4. Prompt del usuario: agregar la US al archivo de epics e historias

El usuario pidió:

```text
@pm agrega la us al archivo d:\repos\bmad-skyRoute\_bmad-output\planning-artifacts\epics-and-stories.md
```

## 5. Resultado del agente

El agente leyó el archivo:

```text
_bmad-output/planning-artifacts/epics-and-stories.md
```

Luego agregó al final del documento una nueva épica:

```text
EPIC BE-6 — Estabilización del flujo de booking
```

Objetivo:

```text
corregir errores de servidor que impiden completar el flujo de reserva end-to-end.
```

También agregó la User Story:

```text
US BE-6.1 — Corregir crash de validación decimal por cultura en BookingsController
```

Estado:

```text
Ready for dev
```

Referencia al story file:

```text
_bmad-output/implementation-artifacts/1-1-fix-decimal-range-culture-crash.md
```

La US agregada al archivo de epics incluye:

- Contexto técnico del bug.
- Criterios de aceptación.
- Tareas de implementación.
- Referencia al archivo detallado de la story.

## 6. Estado final del proceso

Artefactos creados o modificados durante el chat:

- Creado: `_bmad-output/implementation-artifacts/1-1-fix-decimal-range-culture-crash.md`
- Modificado: `_bmad-output/planning-artifacts/epics-and-stories.md`
- Creado: `docs/chatsIA/Definicion de US y Bugs - estabilización.md`

La estabilización quedó documentada como una nueva US lista para desarrollo. El siguiente paso natural es implementar el fix en `FlightSnapshotDto.cs` y agregar los tests de validación correspondientes.
