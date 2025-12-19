using Application.Catalogos.Dtos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Queries;

public sealed record GetBarriosQuery() : IRequest<Result<IReadOnlyList<CatalogoDto>>>;

public sealed class GetBarriosHandler(AppDbContext db) : IRequestHandler<GetBarriosQuery, Result<IReadOnlyList<CatalogoDto>>>
{
  public async Task<Result<IReadOnlyList<CatalogoDto>>> Handle(GetBarriosQuery request, CancellationToken ct)
  {
    var list = await db.Barrios
      .AsNoTracking()
      .OrderBy(x => x.Nombre)
      .Select(x => new CatalogoDto(x.Id, x.Nombre))
      .ToListAsync(ct);

    return Result<IReadOnlyList<CatalogoDto>>.Ok(list);
  }
}
