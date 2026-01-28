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
  bool IsCoordinador,
  string Familia,
  int BarrioId,
  int CodigoCId,
  List<int> LenguasIds,
  int? LiderId,
  int? CoordinadorId,
  MesaVotacionDto MesaVotacion,
  List<int>? CodigosBIds,
  List<int> PersonasACargoIds,
  DateTime CreatedAt,
  string? CreatedBy,
  DateTime? LastModifiedAt,
  string? LastModifiedBy,
  bool VerfAdres,
  bool VerfPuestoVotacion
);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetPersonaByIdQueryHandler(AppDbContext db) : IRequestHandler<GetPersonaByIdQuery, Result<PersonaDto?>>
{
  public async Task<Result<PersonaDto?>> Handle(GetPersonaByIdQuery request, CancellationToken cancellationToken)
  {
    return await db.Personas
      .AsNoTracking()
      .AsSplitQuery()
      .Include(p => p.CodigosB!)
      .Include(p => p.PersonasACargo)
      .Include(p => p.Lenguas!)
      .Include(p => p.User)
      .Where(p => p.Id == request.Id)
      .Select(p => new PersonaDto(
        p.Id,
        p.Nombre,
        p.Apellido,
        p.Cedula,
        p.Apodo,
        p.Telefono ?? string.Empty,
        p.Direccion ?? string.Empty,
        p.Descripcion ?? string.Empty,
        p.IsLider,
        p.IsCoordinador,
        p.Familia ?? string.Empty,
        p.BarrioId ?? 0,
        p.CodigoCId,
        p.Lenguas != null ? p.Lenguas.Select(l => l.LenguaId).ToList() : new List<int>(),
        p.LiderId,
        p.CoordinadorId,
        p.MesaVotacion != null ?
            new MesaVotacionDto(
                p.MesaVotacion.Id,
                p.MesaVotacion.Nombre ?? string.Empty,
                p.MesaVotacion.PuestoVotacion != null ?
                    new PuestoVotacionDto(
                        p.MesaVotacion.PuestoVotacion.Id,
                        p.MesaVotacion.PuestoVotacion.Nombre ?? string.Empty
                    ) :
                    new PuestoVotacionDto(0, string.Empty)
            ) :
            new MesaVotacionDto(0, string.Empty, new PuestoVotacionDto(0, string.Empty)),
        p.CodigosB != null ? p.CodigosB.Select(cb => cb.CodigoBId).ToList() : new List<int>(),
        p.PersonasACargo != null ? p.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>(),
        p.CreatedAt,
        db.Users.Where(u => u.Id.ToString() == p.CreatedByUserId).Select(u => (u.UserName ?? string.Empty).ToUpper()).FirstOrDefault() ?? string.Empty,
        p.LastModifiedAt,
        db.Users.Where(u => u.Id.ToString() == p.LastModifiedByUserId).Select(u => (u.UserName ?? string.Empty).ToUpper()).FirstOrDefault() ?? string.Empty,
        p.VerfAdres,
        p.VerfPuestoVotacion
      ))
      .FirstOrDefaultAsync(cancellationToken) is PersonaDto dto
        ? Result<PersonaDto?>.Ok(dto)
        : Result<PersonaDto?>.Fail(Error.NotFound("Persona no encontrada.", "Persona.Get.NotFound"));
  }
}
