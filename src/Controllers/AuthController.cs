
using Application.Admin.Commands;
using Application.Auth.Commands;
using Application.Auth.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;


[ApiController]
[Route("api/auth")]
public class AuthController(IMediator mediator) : ControllerBase
{
  [HttpGet("usuarios")]
  public async Task<IActionResult> GetUsuarios(CancellationToken ct)
  {
    var result = await mediator.Send(new GetUserQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpPut("usuario/{id:guid}/rol")]
  public async Task<IActionResult> ChangeUserRole(Guid id, [FromBody] ChangeUserRoleCommand command, CancellationToken ct)
  {
    command = command with { UserId = id };
    var result = await mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPut("usuario/{id:guid}/recovery")]
  public async Task<IActionResult> RecoveryUsuario(Guid id, CancellationToken ct)
  {
    var result = await mediator.Send(new RecoveryUserCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login(LoginCommand command, CancellationToken ct)
  {
    var result = await mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPut("usuario/{id:guid}/toggle")]
  public async Task<IActionResult> ToggleUsuario(Guid id, CancellationToken ct)
  {
    var result = await mediator.Send(new ToggleUsuarioCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register(CreateUserCommand command, CancellationToken ct)
  {
    var result = await mediator.Send(command, ct);
    return result.ToActionResult(this);
  }
}