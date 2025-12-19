using Domain.Catalogos;

namespace Domain.Personas;

public class PersonaLengua
{
  public int PersonaId { get; set; }
  public Persona Persona { get; set; } = null!;

  public int LenguaId { get; set; }
  public Lengua Lengua { get; set; } = null!;
}