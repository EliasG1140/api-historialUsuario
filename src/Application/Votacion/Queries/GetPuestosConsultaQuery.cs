using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Votacion.Queries;

public sealed record GetPuestosConsultaQuery : IRequest<Result<PuestosConsultaDto>>;

public sealed record PuestosConsultaDto(
    List<PuestoVotacionDtoItem> PuestosDeVotacion,
    List<MesaVotacionDtoItem> MesasDeVotacion
);

public sealed record PuestoVotacionDtoItem(
    int Id,
    string Nombre,
    int CantidadPersonas,
    int CantidadMesas
);

public sealed record MesaVotacionDtoItem(
    int Id,
    string Nombre,
    int CantidadPersonas,
    int PuestoVotacionId
);

public sealed class GetPuestosConsultaQueryHandler : IRequestHandler<GetPuestosConsultaQuery, Result<PuestosConsultaDto>>
{
  private readonly AppDbContext _db;
  public GetPuestosConsultaQueryHandler(AppDbContext db)
  {
    _db = db;
  }

  public async Task<Result<PuestosConsultaDto>> Handle(GetPuestosConsultaQuery request, CancellationToken cancellationToken)
  {
    var puestos = await _db.PuestosVotacion
        .Include(p => p.MesasVotacion)
            .ThenInclude(m => m.Personas)
        .Select(p => new PuestoVotacionDtoItem(
            p.Id,
            p.Nombre,
            p.MesasVotacion.SelectMany(m => m.Personas).Count(),
            p.MesasVotacion.Count()
        ))
        .ToListAsync(cancellationToken);

    var mesas = await _db.MesasVotacion
        .Include(m => m.Personas)
        .Select(m => new MesaVotacionDtoItem(
            m.Id,
            m.Nombre,
            m.Personas.Count(),
            m.PuestoVotacionId
        ))
        .ToListAsync(cancellationToken);

    var result = new PuestosConsultaDto(puestos, mesas);
    return Result<PuestosConsultaDto>.Ok(result);
  }
}
