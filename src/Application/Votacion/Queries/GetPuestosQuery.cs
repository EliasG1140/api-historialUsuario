using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Votacion.Queries;

//* ------------------------------- Query ------------------------------- */

public sealed record GetPuestosQuery() : IRequest<List<PuestoVotacionDto>>;
public sealed record PuestoVotacionDto(int Id, string Nombre, List<MesaVotacionDto> Mesas);
public sealed record MesaVotacionDto(int Id, string Nombre);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetPuestosQueryHandler(AppDbContext db) : IRequestHandler<GetPuestosQuery, List<PuestoVotacionDto>>
{
  public async Task<List<PuestoVotacionDto>> Handle(GetPuestosQuery request, CancellationToken cancellationToken)
  {
    return await db.PuestosVotacion
      .Include(p => p.MesasVotacion)
      .OrderBy(p => p.Nombre)
      .Select(p => new PuestoVotacionDto(
        p.Id,
        p.Nombre,
        p.MesasVotacion.Select(m => new MesaVotacionDto(m.Id, m.Nombre)).ToList()
      ))
      .ToListAsync(cancellationToken);
  }
}
