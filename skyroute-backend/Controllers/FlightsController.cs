using Microsoft.AspNetCore.Mvc;
using SkyRoute.Api.DTOs;
using SkyRoute.Api.Interfaces.IServices;

namespace SkyRoute.Api.Controllers;

[ApiController]
[Route("api/flights")]
public sealed class FlightsController(IFlightSearchService flightSearchService) : ControllerBase
{
    [HttpPost("search")]
    [ProducesResponseType(typeof(FlightSearchResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FlightSearchResponseDto>> Search(
        [FromBody] FlightSearchRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var response = await flightSearchService.SearchAsync(request, cancellationToken);
        return Ok(response);
    }
}
