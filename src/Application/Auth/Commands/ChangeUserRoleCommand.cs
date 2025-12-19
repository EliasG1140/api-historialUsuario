using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record ChangeUserRoleCommand(Guid UserId, string NewRole) : IRequest<Result<string>>;

//* ------------------------------- Handler ------------------------------- */
public sealed class ChangeUserRoleCommandHandler : IRequestHandler<ChangeUserRoleCommand, Result<string>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public ChangeUserRoleCommandHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<string>> Handle(ChangeUserRoleCommand request, CancellationToken cancellationToken)
  {
    var newRole = request.NewRole?.Trim();

    if (string.IsNullOrWhiteSpace(newRole))
      return Result<string>.Fail(Error.Validation("user.role.empty", "El rol no puede ser vacío."));

    var user = await _userManager.FindByIdAsync(request.UserId.ToString());
    if (user is null)
      return Result<string>.Fail(Error.NotFound("user.notfound", "No se encontró el usuario especificado."));

    if (!await _db.Roles.AnyAsync(r => r.Name == newRole, cancellationToken))
      return Result<string>.Fail(Error.NotFound("role.notfound", $"El rol '{newRole}' no existe."));

    var currentRoles = await _userManager.GetRolesAsync(user);

    if (currentRoles.Any())
    {
      var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
      if (!removeResult.Succeeded)
        return Result<string>.Fail(Error.Conflict("user.role.remove",
          string.Join("; ", removeResult.Errors.Select(e => e.Description))));
    }

    var addResult = await _userManager.AddToRoleAsync(user, newRole);
    if (!addResult.Succeeded)
      return Result<string>.Fail(Error.Conflict("user.role.add",
        string.Join("; ", addResult.Errors.Select(e => e.Description))));

    return Result<string>.Ok(newRole);
  }
}
