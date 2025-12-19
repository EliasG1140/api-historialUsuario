using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetLideresQuery : IRequest<Result<List<LiderListDto>>>;

public sealed record LiderListDto(
    int Id,
    string Nombre,
    string Apellido,
    string Cedula,
    string? Apodo,
    string Telefono,
    string Direccion,
    string? Descripcion,
    bool IsLider,
    BarrioDto Barrio,
    CodigoCDto CodigoC,
    List<LenguaDto> Lenguas,
    int? LiderId,
    MesaVotacionDto MesaVotacion,
    List<CodigoBDto> CodigosB,
    List<int> PersonasACargoIds,
    int PersonasACargoCount
);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetLideresQueryHandler(AppDbContext db) : IRequestHandler<GetLideresQuery, Result<List<LiderListDto>>>
{
  public async Task<Result<List<LiderListDto>>> Handle(GetLideresQuery request, CancellationToken cancellationToken)
  {
    var lideres = await db.Personas
        .Where(p => p.IsLider)
        .Include(p => p.CodigosB!)
            .ThenInclude(cb => cb.CodigoB!)
        .Include(p => p.PersonasACargo)
        .Include(p => p.Lenguas!)
            .ThenInclude(l => l.Lengua!)
        .Include(p => p.Barrio!)
        .Include(p => p.CodigoC!)
        .Include(p => p.MesaVotacion!)
            .ThenInclude(mv => mv.PuestoVotacion!)
        .Select(p => new LiderListDto(
            p.Id,
            p.Nombre,
            p.Apellido,
            p.Cedula,
            p.Apodo,
            p.Telefono,
            p.Direccion,
            p.Descripcion,
            p.IsLider,
            new BarrioDto(p.Barrio!.Id, p.Barrio.Nombre),
            new CodigoCDto(p.CodigoC!.Id, p.CodigoC.Nombre),
            p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua!.Id, l.Lengua.Nombre)).ToList() : new List<LenguaDto>(),
            p.LiderId,
            new MesaVotacionDto(
                p.MesaVotacion!.Id,
                p.MesaVotacion.Nombre,
                new PuestoVotacionDto(
                    p.MesaVotacion.PuestoVotacion!.Id,
                    p.MesaVotacion.PuestoVotacion.Nombre
                )
            ),
            p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB!.Id, cb.CodigoB.Nombre)).ToList() : new List<CodigoBDto>(),
            p.PersonasACargo != null ? p.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>(),
            p.PersonasACargo != null ? p.PersonasACargo.Count : 0
        ))
        .ToListAsync(cancellationToken);

    return Result<List<LiderListDto>>.Ok(lideres);
  }
}
