using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetPersonasQuery(
  int? LiderId = null,
  bool? Lideres = null,
  int? PuestoVotacionId = null,
  int? MesaVotacionId = null,
  int? CodigoBId = null,
  int? CodigoCId = null,
  int? CategoriaId = null
) : IRequest<Result<List<PersonaListDto>>>;

public sealed record PersonaListDto(
    int Id,
    string Nombre,
    string Apellido,
    string Cedula,
    string? Apodo,
    string Telefono,
    string Direccion,
    string? Descripcion,
    bool IsLider,
    BarrioDto Barrio,
    CodigoCDto CodigoC,
    List<LenguaDto> Lenguas,
    int? LiderId,
    MesaVotacionDto MesaVotacion,
    List<CodigoBDto> CodigosB,
    List<int> PersonasACargoIds
);

public sealed record LenguaDto(int Id, string Nombre);
public sealed record CodigoBDto(int Id, string Nombre);
public sealed record CodigoCDto(int Id, string Nombre);
public sealed record BarrioDto(int Id, string Nombre);
public sealed record MesaVotacionDto(int Id, string Nombre, PuestoVotacionDto PuestoVotacion);
public sealed record PuestoVotacionDto(int Id, string Nombre);

//* ------------------------------ Handler ------------------------------ */
public sealed class GetPersonasQueryHandler(AppDbContext db) : IRequestHandler<GetPersonasQuery, Result<List<PersonaListDto>>>
{
  public async Task<Result<List<PersonaListDto>>> Handle(GetPersonasQuery request, CancellationToken cancellationToken)
  {

    var query = db.Personas
      .Include(p => p.CodigosB!)
        .ThenInclude(cb => cb.CodigoB!)
      .Include(p => p.PersonasACargo)
      .Include(p => p.Lenguas!)
        .ThenInclude(l => l.Lengua!)
      .Include(p => p.Barrio!)
      .Include(p => p.CodigoC!)
      .Include(p => p.MesaVotacion!)
        .ThenInclude(mv => mv.PuestoVotacion!)
      .OrderBy(p => p.Apellido)
      .ThenBy(p => p.Nombre)
      .AsQueryable();


    // Filtro por lideres (tiene prioridad sobre lider individual)
    if (request.Lideres.HasValue)
    {
      query = query.Where(p => p.IsLider == request.Lideres.Value);

      // Si solo queremos lideres, no filtramos por LiderId

      // Lógica de categoría solo si Lideres == true y CategoriaId tiene valor
      if (request.Lideres.Value && request.CategoriaId.HasValue)
      {
        // Obtener la categoría
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == request.CategoriaId.Value, cancellationToken);
        if (categoria != null)
        {
          // Filtrar líderes por cantidad de personas a cargo dentro del rango de la categoría
          query = query.Where(p => p.PersonasACargo.Count >= categoria.Minimo && p.PersonasACargo.Count <= categoria.Maximo);
        }
      }
    }
    else if (request.LiderId.HasValue)
    {
      query = query.Where(p => p.LiderId == request.LiderId.Value);
    }

    // Filtro por puesto de votación
    if (request.PuestoVotacionId.HasValue)
    {
      query = query.Where(p => p.MesaVotacion.PuestoVotacionId == request.PuestoVotacionId.Value);
    }

    // Filtro por mesa de votación
    if (request.MesaVotacionId.HasValue)
    {
      query = query.Where(p => p.MesaVotacionId == request.MesaVotacionId.Value);
    }

    // Filtro por CodigoB
    if (request.CodigoBId.HasValue)
    {
      query = query.Where(p => p.CodigosB!.Any(cb => cb.CodigoBId == request.CodigoBId.Value));
    }

    // Filtro por CodigoC
    if (request.CodigoCId.HasValue)
    {
      query = query.Where(p => p.CodigoCId == request.CodigoCId.Value);
    }

    var personas = await query
        .Select(p => new PersonaListDto(
            p.Id,
            p.Nombre,
            p.Apellido,
            p.Cedula,
            p.Apodo,
            p.Telefono,
            p.Direccion,
            p.Descripcion,
            p.IsLider,
            new BarrioDto(p.Barrio!.Id, p.Barrio.Nombre),
            new CodigoCDto(p.CodigoC!.Id, p.CodigoC.Nombre),
            p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua!.Id, l.Lengua.Nombre)).ToList() : new List<LenguaDto>(),
            p.LiderId,
            new MesaVotacionDto(
                p.MesaVotacion!.Id,
                p.MesaVotacion.Nombre,
                new PuestoVotacionDto(
                    p.MesaVotacion.PuestoVotacion!.Id,
                    p.MesaVotacion.PuestoVotacion.Nombre
                )
            ),
            p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB!.Id, cb.CodigoB.Nombre)).ToList() : new List<CodigoBDto>(),
            p.PersonasACargo != null ? p.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>()
        ))
        .ToListAsync(cancellationToken);

    return Result<List<PersonaListDto>>.Ok(personas);
  }
}
