using Domain.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Auth;

public class AppUserConfig : IEntityTypeConfiguration<AppUser>
{
  public void Configure(EntityTypeBuilder<AppUser> b)
  {
    b.ToTable("user");

    b.Property(u => u.UserName)
      .HasMaxLength(256)
      .IsRequired();

    b.Property(u => u.NormalizedUserName)
      .HasMaxLength(256)
      .IsRequired();

    b.HasOne(x => x.Persona)
      .WithOne(t => t.User)
      .HasForeignKey<AppUser>(x => x.PersonaId)
      .OnDelete(DeleteBehavior.Restrict)
      .HasConstraintName("FK_user_persona");

    b.HasIndex(u => u.PersonaId)
      .IsUnique()
      .HasFilter("\"PersonaId\" IS NOT NULL");

    //* -------------------------------------------------------------------------- */
    //*                                Default User                                */
    //* -------------------------------------------------------------------------- */
    var defaultUserId = Guid.Parse("96cb56e3-def8-4433-8341-ecf5424c7a7f");
    var defaultUser = new AppUser
    {
      Id = defaultUserId,
      UserName = "admin",
      NormalizedUserName = "ADMIN",
      Email = "admin@example.com",
      NormalizedEmail = "ADMIN@EXAMPLE.COM",
      EmailConfirmed = true,
      SecurityStamp = "d21bd88e-7f24-4fb9-b9f2-34924342ef90",
      ConcurrencyStamp = "16de0733-137a-4fa7-b65e-7bfc548e2be3",
      PasswordHash = "AQAAAAIAAYagAAAAEAKqLP59ArNX3MAlKYFmKMlc2Rc8QJDlgQL/CWRVH63Cg4Z1JYLKTP9BTUeJOTnMuA==" // 123123123

    };

    b.HasData(defaultUser);
  }
}