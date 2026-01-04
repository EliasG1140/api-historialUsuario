using Domain.Auth;
using Domain.Catalogos;

namespace Domain.Personas;

public class Persona
{
  public int Id { get; set; }
  public string Nombre { get; set; } = null!;
  public string Apellido { get; set; } = null!;
  public string Cedula { get; set; } = null!;
  public string? Apodo { get; set; }
  public string Telefono { get; set; } = null!;
  public string? Direccion { get; set; } = null!;
  public string? Descripcion { get; set; }
  public bool IsLider { get; set; }

  public int? BarrioId { get; set; }
  public Barrio? Barrio { get; set; } = null!;

  public int CodigoCId { get; set; }
  public CodigoC CodigoC { get; set; } = null!;

  /* ------------------------------- Auto - Ref ------------------------------- */
  public int? LiderId { get; set; }
  public Persona? Lider { get; set; }
  public ICollection<Persona> PersonasACargo { get; set; } = new List<Persona>();

  /* -------------------------------- Relations ------------------------------- */
  public AppUser? User { get; set; }

  public int MesaVotacionId { get; set; }
  public MesaVotacion MesaVotacion { get; set; } = null!;
  public ICollection<PersonaCodigoB>? CodigosB { get; set; } = new List<PersonaCodigoB>();
  public ICollection<PersonaLengua>? Lenguas { get; set; } = new List<PersonaLengua>();

  // Auditoría
  public DateTime CreatedAt { get; set; }
  public string? CreatedByUserId { get; set; }
  public DateTime? LastModifiedAt { get; set; }
  public string? LastModifiedByUserId { get; set; }

}