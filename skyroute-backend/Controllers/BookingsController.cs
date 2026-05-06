using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Exceptions;
using SkyRoute.Api.Interfaces.IServices;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/bookings")]
public sealed class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(CreateBookingResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CreateBookingResponseDto>> Create(
        [FromBody] CreateBookingRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var response = await bookingService.CreateAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (DocumentRuleViolationException ex)
        {
            ModelState.AddModelError(nameof(CreateBookingRequestDto.Passenger), ex.Message);
            return ValidationProblem(ModelState);
        }
        catch (BookingSaveFailedException)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
