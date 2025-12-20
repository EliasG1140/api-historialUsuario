using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetPersonaByIdQuery(int Id) : IRequest<Result<PersonaDto?>>;

public sealed record PersonaDto(
  int Id,
  string Nombre,
  string Apellido,
  string Cedula,
  string? Apodo,
  string Telefono,
  string Direccion,
  string? Descripcion,
  bool IsLider,
  int BarrioId,
  int CodigoCId,
  List<int> LenguasIds,
  int? LiderId,
  MesaVotacionDto MesaVotacion,
  List<int>? CodigosBIds,
  List<int> PersonasACargoIds,
  DateTime CreatedAt,
  string? CreatedBy,
  DateTime? LastModifiedAt,
  string? LastModifiedBy
);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetPersonaByIdQueryHandler(AppDbContext db) : IRequestHandler<GetPersonaByIdQuery, Result<PersonaDto?>>
{
  public async Task<Result<PersonaDto?>> Handle(GetPersonaByIdQuery request, CancellationToken cancellationToken)
  {
    return await db.Personas
      .Include(p => p.CodigosB)
      .Include(p => p.PersonasACargo)
      .Include(p => p.Lenguas)
      .Include(p => p.User)
      .Where(p => p.Id == request.Id)
      .Select(p => new PersonaDto(
        p.Id,
        p.Nombre,
        p.Apellido,
        p.Cedula,
        p.Apodo,
        p.Telefono,
        p.Direccion,
        p.Descripcion,
        p.IsLider,
        p.BarrioId,
        p.CodigoCId,
        p.Lenguas.Select(l => l.LenguaId).ToList(),
        p.LiderId,
         new MesaVotacionDto(
                p.MesaVotacion.Id,
                p.MesaVotacion.Nombre,
                new PuestoVotacionDto(
                    p.MesaVotacion.PuestoVotacion.Id,
                    p.MesaVotacion.PuestoVotacion.Nombre
                )
            ),
        p.CodigosB != null ? p.CodigosB.Select(cb => cb.CodigoBId).ToList() : new List<int>(),
        p.PersonasACargo.Select(pa => pa.Id).ToList(),
        p.CreatedAt,
        db.Users.Where(u => u.Id.ToString() == p.CreatedByUserId).Select(u => (u.UserName ?? "").ToUpper()).FirstOrDefault(),
        p.LastModifiedAt,
        db.Users.Where(u => u.Id.ToString() == p.LastModifiedByUserId).Select(u => (u.UserName ?? "").ToUpper()).FirstOrDefault()
      ))
      .FirstOrDefaultAsync(cancellationToken) is PersonaDto dto
        ? Result<PersonaDto?>.Ok(dto)
        : Result<PersonaDto?>.Fail(Error.NotFound("Persona no encontrada.", "Persona.Get.NotFound"));
  }
}
