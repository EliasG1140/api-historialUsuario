using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Catalogos.Queries;

/* ---------------------------------- Query --------------------------------- */
public sealed record GetPersonaLiderQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

/* --------------------------------- Handler -------------------------------- */
public sealed class GetPersonaLiderQueryHandler(AppDbContext db) : IRequestHandler<GetPersonaLiderQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetPersonaLiderQuery request, CancellationToken cancellationToken)
  {
    var list = await db.Personas
        .Where(p => p.IsLider)
        .Select(p => new CatalogoDto(p.Id, p.Nombre + " " + p.Apellido))
        .ToListAsync(cancellationToken);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
