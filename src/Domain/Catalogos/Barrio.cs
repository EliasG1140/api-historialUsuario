using Domain.Personas;

namespace Domain.Catalogos;

public class Barrio
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  public ICollection<Persona> Personas { get; set; } = new List<Persona>();
}