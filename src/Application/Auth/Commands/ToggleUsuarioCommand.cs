using Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Admin.Commands;

//* --------------------------------- Command -------------------------------- */
public sealed record ToggleUsuarioCommand(Guid UsuarioId) : IRequest<Result<ToggleUsuarioResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record ToggleUsuarioResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class ToggleUsuarioHandler(UserManager<AppUser> userManager) : IRequestHandler<ToggleUsuarioCommand, Result<ToggleUsuarioResponse>>
{
  private readonly UserManager<AppUser> _userManager = userManager;

  public async Task<Result<ToggleUsuarioResponse>> Handle(ToggleUsuarioCommand cmd, CancellationToken ct)
  {
    var user = await _userManager.FindByIdAsync(cmd.UsuarioId.ToString());
    if (user is null)
      return Result<ToggleUsuarioResponse>.Fail(Error.NotFound("usuario.notfound", "Usuario no encontrado."));

    if (!user.LockoutEnabled)
    {
      var enabled = await _userManager.SetLockoutEnabledAsync(user, true);
      if (!enabled.Succeeded)
        return Result<ToggleUsuarioResponse>.Fail(Error.Conflict("usuario.lockout", "No se pudo habilitar el lockout del usuario."));
    }

    var lockedOut = await _userManager.IsLockedOutAsync(user);

    if (lockedOut)
    {
      await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddMinutes(-1));
      await _userManager.ResetAccessFailedCountAsync(user);

      return Result<ToggleUsuarioResponse>.Ok(new ToggleUsuarioResponse("Usuario desbloqueado correctamente."));
    }
    else
    {
      await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));

      return Result<ToggleUsuarioResponse>.Ok(new ToggleUsuarioResponse("Usuario bloqueado correctamente."));
    }
  }
}