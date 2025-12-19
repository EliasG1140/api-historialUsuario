using Domain.Personas;

namespace Domain.Catalogos;

public class RegistroMesa
{
  public int Id { get; set; }

  /* -------------------------------- Relations ------------------------------- */
  public int PersonaId { get; set; }
  public Persona Persona { get; set; } = null!;

  public int MesaVotacionId { get; set; }
  public MesaVotacion MesaVotacion { get; set; } = null!;
}