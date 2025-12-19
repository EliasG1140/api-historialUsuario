
using Domain.Catalogos;

namespace Domain.Personas;

public class PersonaCodigoB
{
  public int PersonaId { get; set; }
  public Persona Persona { get; set; } = null!;

  public int CodigoBId { get; set; }
  public CodigoB CodigoB { get; set; } = null!;
}