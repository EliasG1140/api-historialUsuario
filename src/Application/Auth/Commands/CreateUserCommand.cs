using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record CreateUserCommand(string UserName, string Role, string Cedula) : IRequest<Result<CreateUserResponse>>;

//* ------------------------------- Handler ------------------------------- */
public sealed record CreateUserResponse(Guid UserId, string Password);

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<CreateUserResponse>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public CreateUserCommandHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<CreateUserResponse>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
  {
    AppUser user;
    var userNameUpper = request.UserName.ToUpperInvariant();
    if (request.Role.ToUpperInvariant() == "ADMINISTRADOR")
    {
      user = new AppUser
      {
        UserName = userNameUpper
      };
    }
    else if (request.Role.ToUpperInvariant() == "DIGITALIZADOR")
    {
      var persona = await _db.Personas.FirstOrDefaultAsync(p => p.Cedula == request.Cedula, cancellationToken);
      if (persona == null)
      {
        return Result<CreateUserResponse>.Fail(Error.NotFound("No se encontró una persona registrada con la cédula especificada.", "persona.notfound"));
      }

      var existeUsuario = await _db.Users.AnyAsync(u => u.PersonaId == persona.Id, cancellationToken);
      if (existeUsuario)
      {
        return Result<CreateUserResponse>.Fail(Error.Conflict("La persona ya tiene una cuenta de usuario asociada.", "user.persona.exists"));
      }

      user = new AppUser
      {
        UserName = userNameUpper,
        PersonaId = persona.Id
      };
    }
    else
    {
      return Result<CreateUserResponse>.Fail(Error.Validation("Rol no soportado para la creación de usuario.", "user.role.unsupported"));
    }

    // Generar contraseña aleatoria de 6 caracteres alfanuméricos en mayúsculas
    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    var generatedPassword = new string(Enumerable.Repeat(chars, 6)
      .Select(s => s[random.Next(s.Length)]).ToArray());

    var result = await _userManager.CreateAsync(user, generatedPassword);
    if (!result.Succeeded)
    {
      var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
      return Result<CreateUserResponse>.Fail(Error.Conflict(errorMsg, "user.create"));
    }

    var roleResult = await _userManager.AddToRoleAsync(user, request.Role);
    if (!roleResult.Succeeded)
    {
      var errorMsg = string.Join("; ", roleResult.Errors.Select(e => e.Description));
      return Result<CreateUserResponse>.Fail(Error.Conflict(errorMsg, "user.role"));
    }

    return Result<CreateUserResponse>.Ok(new CreateUserResponse(user.Id, generatedPassword));
  }
}
