using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Queries;

public sealed record GetLenguasQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

public sealed class GetLenguasHandler(AppDbContext db) : IRequestHandler<GetLenguasQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetLenguasQuery request, CancellationToken ct)
  {
    var list = await db.Lenguas
      .AsNoTracking()
      .OrderBy(x => x.Nombre)
      .Select(x => new CatalogoDto(x.Id, x.Nombre))
      .ToListAsync(ct);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
