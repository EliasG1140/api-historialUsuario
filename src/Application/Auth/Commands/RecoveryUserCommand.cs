using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record RecoveryUserCommand(Guid UserId) : IRequest<Result<string>>;

//* ------------------------------- Handler ------------------------------- */
public sealed class RecoveryUserCommandHandler : IRequestHandler<RecoveryUserCommand, Result<string>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public RecoveryUserCommandHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<string>> Handle(RecoveryUserCommand request, CancellationToken cancellationToken)
  {
    var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
    if (user == null)
    {
      return Result<string>.Fail(Error.NotFound("user.notfound", "No se encontró el usuario especificado."));
    }

    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetResult = await _userManager.ResetPasswordAsync(user, token, user.UserName!);
    if (!resetResult.Succeeded)
    {
      var errorMsg = string.Join("; ", resetResult.Errors.Select(e => e.Description));
      return Result<string>.Fail(Error.Conflict("user.recovery", errorMsg));
    }

    return Result<string>.Ok(user.UserName!);
  }
}
