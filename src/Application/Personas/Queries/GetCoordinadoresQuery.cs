using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetCoordinadoresQuery : IRequest<Result<List<CoordinadorListDto>>>;

public sealed record CoordinadorListDto(
    int Id,
    string Nombre,
    string Apellido,
    string Cedula,
    string? Apodo,
    string Telefono,
    string Direccion,
    string? Descripcion,
    bool IsCoordinador,
    BarrioDto Barrio,
    CodigoCDto CodigoC,
    List<LenguaDto> Lenguas,
    int? CoordinadorId,
    MesaVotacionDto MesaVotacion,
    List<CodigoBDto> CodigosB,
    List<int> LideresIds,
    int LideresCount
);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetCoordinadoresQueryHandler(AppDbContext db) : IRequestHandler<GetCoordinadoresQuery, Result<List<CoordinadorListDto>>>
{
    public async Task<Result<List<CoordinadorListDto>>> Handle(GetCoordinadoresQuery request, CancellationToken cancellationToken)
    {
        var coordinadores = await db.Personas
            .AsNoTracking()
            .AsSplitQuery()
            .Where(p => p.IsCoordinador)
            .Include(p => p.CodigosB!)
                .ThenInclude(cb => cb.CodigoB!)
            .Include(p => p.Coordinados)
            .Include(p => p.Lenguas!)
                .ThenInclude(l => l.Lengua!)
            .Include(p => p.Barrio!)
            .Include(p => p.CodigoC!)
            .Include(p => p.MesaVotacion!)
                .ThenInclude(mv => mv.PuestoVotacion!)
            .Select(p => new CoordinadorListDto(
                p.Id,
                p.Nombre,
                p.Apellido,
                p.Cedula,
                p.Apodo,
                p.Telefono ?? string.Empty,
                p.Direccion ?? string.Empty,
                p.Descripcion ?? string.Empty,
                p.IsCoordinador,
                p.Barrio != null ? new BarrioDto(p.Barrio.Id, p.Barrio.Nombre ?? string.Empty) : new BarrioDto(0, string.Empty),
                p.CodigoC != null ? new CodigoCDto(p.CodigoC.Id, p.CodigoC.Nombre ?? string.Empty) : new CodigoCDto(0, string.Empty),
                p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua.Id, l.Lengua.Nombre ?? string.Empty)).ToList() : new List<LenguaDto>(),
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
                p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB.Id, cb.CodigoB.Nombre ?? string.Empty)).ToList() : new List<CodigoBDto>(),
                p.Coordinados != null ? p.Coordinados.Select(l => l.Id).ToList() : new List<int>(),
                p.Coordinados != null ? p.Coordinados.Count : 0
            ))
            .ToListAsync(cancellationToken);

        return Result<List<CoordinadorListDto>>.Ok(coordinadores);
    }
}
