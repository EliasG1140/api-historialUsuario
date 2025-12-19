
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Home.Queries;

public sealed record GetCategorizacionQuery() : IRequest<Result<List<GetCategorizacion>>>;

public sealed record GetCategorizacion(
  int Id,
  string Nombre,
  int Minimo,
  int Maximo,
  int Count,
  int Total
);

public sealed class GetCategorizacionHandler(AppDbContext db) : IRequestHandler<GetCategorizacionQuery, Result<List<GetCategorizacion>>>
{
  public async Task<Result<List<GetCategorizacion>>> Handle(GetCategorizacionQuery request, CancellationToken ct)
  {
    var categorias = await db.Categorias.ToListAsync(ct);

    var lideres = await db.Personas
      .Where(p => p.IsLider)
      .Select(l => new {
        l.Id,
        PersonasACargoCount = l.PersonasACargo.Count
      })
      .ToListAsync(ct);

    var result = categorias.Select(categoria => {
      var lideresEnCategoria = lideres.Where(l => l.PersonasACargoCount >= categoria.Minimo && l.PersonasACargoCount <= categoria.Maximo).ToList();
      var count = lideresEnCategoria.Count;
      var total = lideresEnCategoria.Sum(l => l.PersonasACargoCount);
      return new GetCategorizacion(
        categoria.Id,
        categoria.Nombre,
        categoria.Minimo,
        categoria.Maximo,
        count,
        total
      );
    }).ToList();

    return Result<List<GetCategorizacion>>.Ok(result);
  }
}