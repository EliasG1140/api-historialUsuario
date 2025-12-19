using Domain.Auth;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Queries;

public sealed record GetUserQuery : IRequest<Result<List<UserDto>>>;

public sealed record UserDto(Guid Id, string UserName, string Nombre, string Apellido, string RolName, bool Inactivo);

public sealed class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<List<UserDto>>>
{
  private readonly UserManager<AppUser> _userManager;
  private readonly AppDbContext _db;

  public GetUserQueryHandler(UserManager<AppUser> userManager, AppDbContext db)
  {
    _userManager = userManager;
    _db = db;
  }

  public async Task<Result<List<UserDto>>> Handle(GetUserQuery request, CancellationToken cancellationToken)
  {
    var excludedId = Guid.Parse("96cb56e3-def8-4433-8341-ecf5424c7a7f");
    var users = await _db.Users
      .Include(u => u.Persona)
      .Where(u => u.Id != excludedId)
      .ToListAsync(cancellationToken);

    var result = new List<UserDto>();
    foreach (var user in users)
    {
      var roles = await _userManager.GetRolesAsync(user);
      result.Add(new UserDto(
          user.Id,
          user.UserName ?? string.Empty,
          user.Persona?.Nombre ?? string.Empty,
          user.Persona?.Apellido ?? string.Empty,
          roles.FirstOrDefault() ?? string.Empty,
          user.LockoutEnd.HasValue && user.LockoutEnd > DateTimeOffset.UtcNow
      ));
    }
    return Result<List<UserDto>>.Ok(result);
  }
}
