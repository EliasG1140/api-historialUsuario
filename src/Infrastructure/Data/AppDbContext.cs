using Domain.Auth;
using Domain.Catalogos;
using Domain.Personas;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
  public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

  //* -------------------------------- Catalogos ------------------------------- */
  public DbSet<CodigoC> CodigosC => Set<CodigoC>();
  public DbSet<CodigoB> CodigosB => Set<CodigoB>();
  public DbSet<Barrio> Barrios => Set<Barrio>();
  public DbSet<Lengua> Lenguas => Set<Lengua>();
  public DbSet<PuestoVotacion> PuestosVotacion => Set<PuestoVotacion>();
  public DbSet<MesaVotacion> MesasVotacion => Set<MesaVotacion>();
  public DbSet<Categoria> Categorias => Set<Categoria>();
  public DbSet<Persona> Personas => Set<Persona>();
  public DbSet<PersonaLengua> PersonasLengua => Set<PersonaLengua>();

  protected override void OnModelCreating(ModelBuilder b)
  {
    base.OnModelCreating(b);

    b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    b.Entity<IdentityUserRole<Guid>>().ToTable("user_role").HasData(
      new IdentityUserRole<Guid>
      {
        UserId = Guid.Parse("96cb56e3-def8-4433-8341-ecf5424c7a7f"),
        RoleId = Guid.Parse("019a4721-233e-78b7-b7af-c71a11a344e4")
      }
    );
    b.Entity<IdentityUserClaim<Guid>>().ToTable("user_claim");
    b.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claim");
    b.Entity<IdentityUserLogin<Guid>>().ToTable("user_login");
    b.Entity<IdentityUserToken<Guid>>().ToTable("user_token");
  }
}