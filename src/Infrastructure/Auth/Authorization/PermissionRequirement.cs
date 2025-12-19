using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Auth.Authorization;

public sealed class PermissionRequirement : IAuthorizationRequirement
{
    public string PermissionCode { get; }
    public PermissionRequirement(string code) => PermissionCode = code;
}
