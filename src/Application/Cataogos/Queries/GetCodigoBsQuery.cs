using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Queries;

public sealed record GetCodigoBsQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

public sealed class GetCodigoBsHandler(AppDbContext db) : IRequestHandler<GetCodigoBsQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetCodigoBsQuery request, CancellationToken ct)
  {
    var list = await db.CodigosB
      .AsNoTracking()
      .OrderBy(x => x.Nombre)
      .Select(x => new CatalogoDto(x.Id, x.Nombre))
      .ToListAsync(ct);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
