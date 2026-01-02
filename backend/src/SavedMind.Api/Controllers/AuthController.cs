using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SavedMind.Api.Contracts.Auth;
using SavedMind.Application.Features.Auth.Register;

namespace SavedMind.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(request.Email, request.Password);
        var result = await mediator.Send(command, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, new
        {
            message = result.Message
        });
    }
}
