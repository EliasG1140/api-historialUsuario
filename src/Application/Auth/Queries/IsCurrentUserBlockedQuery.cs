using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Auth.Queries;

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

/* ---------------------------------- Query --------------------------------- */
public sealed record IsCurrentUserBlockedQuery() : IRequest<Result<bool>>;

/* --------------------------------- Handle --------------------------------- */
public sealed class IsCurrentUserBlockedQueryHandler : IRequestHandler<IsCurrentUserBlockedQuery, Result<bool>>
{
    private readonly AppDbContext _db;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public IsCurrentUserBlockedQueryHandler(AppDbContext db, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Result<bool>> Handle(IsCurrentUserBlockedQuery request, CancellationToken cancellationToken)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user?.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            return Result<bool>.Ok(false);

        var dbUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (dbUser == null)
            return Result<bool>.Ok(false);
        var isBlocked = dbUser.LockoutEnabled && dbUser.LockoutEnd.HasValue && dbUser.LockoutEnd.Value > DateTimeOffset.UtcNow;
        return Result<bool>.Ok(isBlocked);
    }
}
