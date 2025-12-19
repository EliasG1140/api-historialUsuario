using Domain.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Auth.Authorization;

public sealed class PermissionHandler
    : AuthorizationHandler<PermissionRequirement>
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;
    private readonly IMemoryCache _cache;
    private readonly ILogger<PermissionHandler> _logger;

    public PermissionHandler(
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager,
        IMemoryCache cache,
        ILogger<PermissionHandler> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _cache = cache;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context, PermissionRequirement req)
    {
        var userId = _userManager.GetUserId(context.User);
        if (string.IsNullOrEmpty(userId)) return;

        var perms = await _cache.GetOrCreateAsync($"perm:{userId}", async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(2);
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var roleNames = await _userManager.GetRolesAsync(user);
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var roleName in roleNames)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role is null) continue;

                var rclaims = await _roleManager.GetClaimsAsync(role);
                foreach (var c in rclaims.Where(c => c.Type == "permission" && !string.IsNullOrWhiteSpace(c.Value)))
                    set.Add(c.Value);
            }

            return set;
        });

        if ((perms ?? new HashSet<string>()).Contains(req.PermissionCode))
            context.Succeed(req);
        else
            _logger.LogDebug("User {UserId} lacks permission {Permission}", userId, req.PermissionCode);
    }
}
