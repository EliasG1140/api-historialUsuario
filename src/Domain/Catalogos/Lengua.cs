using Domain.Personas;

namespace Domain.Catalogos;

public class Lengua
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  public ICollection<PersonaLengua> PersonasLengua { get; set; } = new List<PersonaLengua>();
}