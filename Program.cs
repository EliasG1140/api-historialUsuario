using System.Security.Claims;
using System.Text;
using Application.Behaviors;
using Application.Common.Tx;
using Domain.Auth;
using FluentValidation;
using Infrastructure.Auth.Authorization;
using Infrastructure.Auth.Jwt;
using Infrastructure.Common.Tx;
using Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configuración QuestPDF
QuestPDF.Settings.License = LicenseType.Community;
QuestPDF.Settings.EnableCaching = true;

builder.Services.AddControllers(); // Añade controladores MVC
builder.Services.AddOpenApi(); // Añade OpenAPI (Swagger)
builder.Services.AddSignalR(); // Añade SignalR

// CORS para tu front (ajusta origen)
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("default", p => p
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithOrigins("http://127.0.0.1:7777", "http://localhost:7777")
        .AllowCredentials());

    opt.AddPolicy("AllowAll", p => p
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// EF Core PostgreSQL
var conn = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<AppDbContext>(
    opt => opt.UseNpgsql(conn),
    contextLifetime: ServiceLifetime.Scoped,
    optionsLifetime: ServiceLifetime.Singleton
);

// Identity Core (sin UI)
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.User.RequireUniqueEmail = false;
    opt.Password.RequireNonAlphanumeric = false;
    opt.Password.RequireLowercase = false;
    opt.Password.RequireUppercase = false;
    opt.Password.RequireDigit = false;
    opt.Password.RequiredLength = 2;

    // Lockout settings
    opt.Lockout.MaxFailedAccessAttempts = int.MaxValue;
    opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.Zero;
})
.AddRoles<AppRole>()
.AddEntityFrameworkStores<AppDbContext>()
.AddSignInManager()
.AddDefaultTokenProviders();

// JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "dev-change-this";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "gov-api";
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services.AddAuthentication(o =>
{
    o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(o =>
    {
        o.MapInboundClaims = false;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience = "Astrevia.Api",

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ClockSkew = TimeSpan.Zero,

            // opcional pero recomendado:
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

// Realtime DI
// builder.Services.AddScoped<IAuthRealtime, AuthRealtime>();
builder.Services.AddScoped<ICommitActions, CommitActions>();

// MediatR + Validations (escanea el assembly actual)
var applicationAssembly = typeof(ValidationBehavior<,>).Assembly;

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(applicationAssembly));
builder.Services.AddValidatorsFromAssembly(applicationAssembly);
builder.Services.AddMemoryCache();

// Pipeline behaviors (Validación y UoW)
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

// Disable automatic model state validation
builder.Services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true);
builder.Services.AddScoped<ExceptionMiddleware>();

// Authorization Handlers
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>(); // Middleware de manejo de excepciones

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll"); // Usa CORS
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); // Mapea controladores MVC
app.Run();