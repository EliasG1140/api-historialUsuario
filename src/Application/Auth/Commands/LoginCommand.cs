using Domain.Auth;
using Infrastructure.Auth.Jwt;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* --------------------------------- Command -------------------------------- */
public sealed record LoginCommand(string UserName, string Password) : IRequest<Result<LoginResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record LoginResponse(string Token, string Nombre, string Apellido, string Cedula, string RolName);

//* --------------------------------- Handler -------------------------------- */
public sealed class LoginCommandHandler(SignInManager<AppUser> signInManager, JwtTokenService jwt, AppDbContext db) : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
  public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
  {
    AppUser? user = await signInManager.UserManager.FindByNameAsync(request.UserName);
    if (user is null)
    {
      return Result<LoginResponse>.Fail(Error.Conflict("validation.credentials", "Nombre de usuario o contraseña incorrecto."));
    }

    var roles = await signInManager.UserManager.GetRolesAsync(user);

    var persona = await db.Personas
      .AsNoTracking()
      .FirstOrDefaultAsync(p => p.Id == user.PersonaId, cancellationToken);

    var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);

    if (result.IsLockedOut)
    {
      return Result<LoginResponse>.Fail(Error.Conflict("validation.credentials", "El usuario se encuentra bloqueado."));
    }

    if (result.Succeeded)
    {
      var token = await jwt.GenerateAsync(user!, cancellationToken);

      var resp = new LoginResponse(token.AccessToken, persona?.Nombre ?? "", persona?.Apellido ?? "", persona?.Cedula ?? "", roles.FirstOrDefault() ?? "");
      return Result<LoginResponse>.Ok(resp);
    }
    else
    {
      return Result<LoginResponse>.Fail(Error.Conflict("validation.credentials", "Nombre de usuario o contraseña incorrecto."));
    }
  }
}