using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Queries;

public sealed record GetCodigoCsQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

public sealed class GetCodigoCsHandler(AppDbContext db) : IRequestHandler<GetCodigoCsQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetCodigoCsQuery request, CancellationToken ct)
  {
    var list = await db.CodigosC
      .AsNoTracking()
      .OrderBy(x => x.Nombre)
      .Select(x => new CatalogoDto(x.Id, x.Nombre))
      .ToListAsync(ct);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
