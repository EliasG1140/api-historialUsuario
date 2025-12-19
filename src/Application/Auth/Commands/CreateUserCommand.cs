using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record CreateUserCommand(string UserName, string Password, string Role, string Cedula) : IRequest<Result<Guid>>;

//* ------------------------------- Handler ------------------------------- */
public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public CreateUserCommandHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {

    var persona = await _db.Personas.FirstOrDefaultAsync(p => p.Cedula == request.Cedula, cancellationToken);
    if (persona == null)
    {
      return Result<Guid>.Fail(Error.NotFound("persona.notfound", "No se encontró una persona registrada con la cédula especificada."));
    }

    var user = new AppUser
    {
      UserName = request.UserName,
      PersonaId = persona.Id
    };

    var result = await _userManager.CreateAsync(user, request.Password);
    if (!result.Succeeded)
    {
      var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
      return Result<Guid>.Fail(Error.Conflict("user.create", errorMsg));
    }

    var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
    if (!roleResult.Succeeded)
    {
      var errorMsg = string.Join("; ", roleResult.Errors.Select(e => e.Description));
      return Result<Guid>.Fail(Error.Conflict("user.role", errorMsg));
    }

    return Result<Guid>.Ok(user.Id);
  }
}
