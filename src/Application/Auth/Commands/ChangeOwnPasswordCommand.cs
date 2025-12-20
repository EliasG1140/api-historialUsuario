using Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Auth.Commands;

public sealed record ChangeOwnPasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Result<string>>;

public sealed class ChangeOwnPasswordCommandHandler : IRequestHandler<ChangeOwnPasswordCommand, Result<string>>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ChangeOwnPasswordCommandHandler(UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<string>> Handle(ChangeOwnPasswordCommand request, CancellationToken cancellationToken)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            return Result<string>.Fail(Error.Unauthorized("No se pudo identificar el usuario autenticado.", "user.unauthorized"));
        }

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<string>.Fail(Error.NotFound("Usuario no encontrado.", "user.notfound"));
        }

        var passwordValid = await _userManager.CheckPasswordAsync(user, request.CurrentPassword);
        if (!passwordValid)
        {
            return Result<string>.Fail(Error.Validation("La contraseña actual es incorrecta.", "user.password.invalid"));
        }

        var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded)
        {
            var errorMsg = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result<string>.Fail(Error.Conflict("No se pudo cambiar la contraseña.", errorMsg));
        }

        return Result<string>.Ok("Contraseña cambiada exitosamente.");
    }
}
