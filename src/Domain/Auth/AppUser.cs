using Domain.Personas;
using Microsoft.AspNetCore.Identity;

namespace Domain.Auth;

public class AppUser : IdentityUser<Guid>
{
  public int? PersonaId { get; set; }
  public Persona? Persona { get; set; }
}