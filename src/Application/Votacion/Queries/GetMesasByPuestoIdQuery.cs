using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Votacion.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetMesasByPuestoIdQuery(int PuestoVotacionId) : IRequest<List<MesaVotacionDto>>;

//* ------------------------------ Handler ------------------------------ */
public sealed class GetMesasByPuestoIdQueryHandler(AppDbContext db) : IRequestHandler<GetMesasByPuestoIdQuery, List<MesaVotacionDto>>
{
  public async Task<List<MesaVotacionDto>> Handle(GetMesasByPuestoIdQuery request, CancellationToken cancellationToken)
  {
    return await db.MesasVotacion
        .Where(m => m.PuestoVotacionId == request.PuestoVotacionId)
        .OrderBy(m => m.Nombre)
        .Select(m => new MesaVotacionDto(m.Id, m.Nombre))
        .ToListAsync(cancellationToken);
  }
}
