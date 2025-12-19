using Domain.Personas;

namespace Domain.Catalogos;

public class MesaVotacion
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  /* -------------------------------- Relations ------------------------------- */
  public int PuestoVotacionId { get; set; }
  public PuestoVotacion PuestoVotacion { get; set; } = null!;

  public ICollection<Persona> Personas { get; set; } = new List<Persona>();
}