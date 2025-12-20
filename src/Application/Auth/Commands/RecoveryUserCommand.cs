using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record RecoveryUserCommand(Guid UserId) : IRequest<Result<ResponseRecoveryUser>>;

/* -------------------------------- Response -------------------------------- */
public sealed record ResponseRecoveryUser(string NewPassword);

//* ------------------------------- Handler ------------------------------- */
public sealed class RecoveryUserCommandHandler : IRequestHandler<RecoveryUserCommand, Result<ResponseRecoveryUser>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public RecoveryUserCommandHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<ResponseRecoveryUser>> Handle(RecoveryUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
    if (user == null)
    {
      return Result<ResponseRecoveryUser>.Fail(Error.NotFound("user.notfound", "No se encontró el usuario especificado."));
    }

    // Generar contraseña aleatoria de 6 caracteres alfanuméricos en mayúsculas
    string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var random = new Random();
    var newPassword = new string(Enumerable.Repeat(chars, 6)
      .Select(s => s[random.Next(s.Length)]).ToArray());

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetResult = await _userManager.ResetPasswordAsync(user, token, newPassword);
    if (!resetResult.Succeeded)
    {
      var errorMsg = string.Join("; ", resetResult.Errors.Select(e => e.Description));
      return Result<ResponseRecoveryUser>.Fail(Error.Conflict("user.recovery", errorMsg));
    }

    return Result<ResponseRecoveryUser>.Ok(new ResponseRecoveryUser(newPassword));
  }
}
