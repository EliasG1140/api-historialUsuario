using Domain.Catalogos;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Queries;

public sealed record GetCategoriasQuery() : IRequest<Result<IReadOnlyList<Categoria>>>;

public sealed class GetCategoriasHandler(AppDbContext db) : IRequestHandler<GetCategoriasQuery, Result<IReadOnlyList<Categoria>>>
{
  public async Task<Result<IReadOnlyList<Categoria>>> Handle(GetCategoriasQuery request, CancellationToken ct)
  {
    var list = await db.Categorias
      .AsNoTracking()
      .OrderBy(x => x.Nombre)
      .ToListAsync(ct);

    return Result<IReadOnlyList<Categoria>>.Ok(list);
  }
}