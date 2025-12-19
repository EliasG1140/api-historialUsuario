namespace Domain.Catalogos;

public class PuestoVotacion
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;

  /* -------------------------------- Relations ------------------------------- */
  public ICollection<MesaVotacion> MesasVotacion { get; set; } = new List<MesaVotacion>();
}