using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Catalogos.Queries;

/* ---------------------------------- Query --------------------------------- */
public sealed record GetPersonaCoordinadorQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

/* --------------------------------- Handler -------------------------------- */
public sealed class GetPersonaCoordinadorQueryHandler(AppDbContext db) : IRequestHandler<GetPersonaCoordinadorQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetPersonaCoordinadorQuery request, CancellationToken cancellationToken)
  {
    var list = await db.Personas
        .AsNoTracking()
        .Where(p => p.IsCoordinador)
        .Select(p => new CatalogoDto(p.Id, $"{p.Cedula} - {p.Nombre} {p.Apellido}{(p.Apodo != null ? $" ({p.Apodo})" : "")}"))
        .ToListAsync(cancellationToken);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
