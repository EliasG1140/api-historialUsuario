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
                p.Direccion ?? string.Empty,
                p.Descripcion ?? string.Empty,
                p.IsLider,
                p.Barrio != null ? new BarrioDto(p.Barrio.Id, p.Barrio.Nombre ?? string.Empty) : new BarrioDto(0, string.Empty),
                p.CodigoC != null ? new CodigoCDto(p.CodigoC.Id, p.CodigoC.Nombre ?? string.Empty) : new CodigoCDto(0, string.Empty),
                p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua.Id, l.Lengua.Nombre ?? string.Empty)).ToList() : new List<LenguaDto>(),
                p.LiderId,
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
                p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB.Id, cb.CodigoB.Nombre ?? string.Empty)).ToList() : new List<CodigoBDto>(),
                p.PersonasACargo != null ? p.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>(),
                p.PersonasACargo != null ? p.PersonasACargo.Count : 0
            ))
            .ToListAsync(cancellationToken);

        return Result<List<LiderListDto>>.Ok(lideres);
    }
}
