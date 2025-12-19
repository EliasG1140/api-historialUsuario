using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Auth.Jwt;

public sealed class JwtTokenService
{
    private readonly IConfiguration _cfg;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public JwtTokenService(
        IConfiguration cfg,
        UserManager<AppUser> userManager,
        RoleManager<AppRole> roleManager)
    {
        _cfg = cfg;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<TokenResult> GenerateAsync(AppUser user, CancellationToken ct = default)
    {
        // Claims base del usuario
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? string.Empty),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        // Claims de usuario (si los usas)
        var userClaims = await _userManager.GetClaimsAsync(user);
        claims.AddRange(userClaims);

        // Roles del usuario como claims (útil si quieres leer roles desde el token)
        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var r in roleNames)
            claims.Add(new Claim(ClaimTypes.Role, r));

        // NOTA importante:
        // No incluimos las "permission" en el JWT para que los cambios
        // se reflejen INMEDIATAMENTE (tu PermissionHandler las consulta
        // desde RoleClaims en la BD en cada autorización, con caché corto).
        //
        // Si quisieras meter permisos en el token (Enfoque A), aquí
        // harías roleManager.GetClaimsAsync(role) y agregas c.Type="permission".

        var keyStr = _cfg["Jwt:Key"];
        if (string.IsNullOrWhiteSpace(keyStr))
            throw new InvalidOperationException("Falta configuración: Jwt:Key");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var issuer = _cfg["Jwt:Issuer"] ?? throw new InvalidOperationException("Falta Jwt:Issuer");
        var audience = _cfg["Jwt:Audience"] ?? throw new InvalidOperationException("Falta Jwt:Audience");
        var minutes = int.TryParse(_cfg["Jwt:AccessTokenMinutes"], out var m) ? m : 60;

        var expires = DateTime.UtcNow.AddMinutes(minutes);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);

        var encoded = new JwtSecurityTokenHandler().WriteToken(token);

        return new TokenResult(encoded, "Bearer", expires, roleNames.ToList());
    }
}

public sealed record TokenResult(string AccessToken, string TokenType, DateTime ExpiresAtUtc, List<string> Roles);
