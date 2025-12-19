using Domain.Personas;

namespace Domain.Catalogos;

public class CodigoB
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  public ICollection<PersonaCodigoB> PersonasCodigoB { get; set; } = new List<PersonaCodigoB>();
}