using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Queries;

//* ------------------------------- Query ------------------------------- */
public sealed record GetPersonasQuery(
  int? LiderId = null,
  int? CoordinadorId = null,
  bool? Lideres = null,
  bool? Coordinadores = null,
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
    bool IsCoordinador,
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
      .AsNoTracking()
      .AsSplitQuery()
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

    // Filtro exclusivo para coordinadores
    if (request.Coordinadores.HasValue && request.Coordinadores.Value)
    {
      query = query.Where(p => p.IsCoordinador);
    }
    // Filtro exclusivo para lideres
    else if (request.Lideres.HasValue && request.Lideres.Value)
    {
      query = query.Where(p => p.IsLider);
      if (request.CoordinadorId.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == request.CoordinadorId.Value);
      }
      if (request.CategoriaId.HasValue)
      {
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == request.CategoriaId.Value, cancellationToken);
        if (categoria != null)
        {
          query = query.Where(p => p.PersonasACargo.Count >= categoria.Minimo && p.PersonasACargo.Count <= categoria.Maximo);
        }
      }
    }
    // Si ninguno de los dos checks está en true, consulta personas en general
    else
    {
      // Si se selecciona un CoordinadorId, trae todas las personas con ese CoordinadorId
      if (request.CoordinadorId.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == request.CoordinadorId.Value);
      }
      // Si se selecciona un LiderId, trae todas las personas con ese LiderId y también incluye al propio líder
      else if (request.LiderId.HasValue)
      {
        int liderId = request.LiderId.Value;
        query = query.Where(p => p.LiderId == liderId || p.Id == liderId);
      }
      // Si no hay filtros, trae todas las personas (no se aplica filtro)
    }


    // Filtro por coordinadores
    if (request.Coordinadores.HasValue && request.Coordinadores.Value)
    {
      query = query.Where(p => p.IsCoordinador);
    }
    // Filtro por lideres (tiene prioridad sobre lider individual)
    else if (request.Lideres.HasValue && request.Lideres.Value)
    {
      query = query.Where(p => p.IsLider);
      // Si se especifica CoordinadorId, filtrar los líderes bajo ese coordinador
      if (request.CoordinadorId.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == request.CoordinadorId.Value);
      }
      // Lógica de categoría solo si Lideres == true y CategoriaId tiene valor
      if (request.CategoriaId.HasValue)
      {
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == request.CategoriaId.Value, cancellationToken);
        if (categoria != null)
        {
          query = query.Where(p => p.PersonasACargo.Count >= categoria.Minimo && p.PersonasACargo.Count <= categoria.Maximo);
        }
      }
    }
    else if (request.LiderId.HasValue)
    {
      query = query.Where(p => p.LiderId == request.LiderId.Value);
    }


    // Filtros adicionales (AND)
    if (request.PuestoVotacionId.HasValue)
    {
      query = query.Where(p => p.MesaVotacion != null && p.MesaVotacion.PuestoVotacionId == request.PuestoVotacionId.Value);
    }
    if (request.MesaVotacionId.HasValue)
    {
      query = query.Where(p => p.MesaVotacionId == request.MesaVotacionId.Value);
    }
    if (request.CodigoBId.HasValue)
    {
      query = query.Where(p => p.CodigosB != null && p.CodigosB.Any(cb => cb.CodigoBId == request.CodigoBId.Value));
    }
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
        p.Telefono ?? string.Empty,
        p.Direccion ?? string.Empty,
        p.Descripcion ?? string.Empty,
        p.IsLider,
        p.IsCoordinador,
        p.Barrio != null ? new BarrioDto(p.Barrio.Id, p.Barrio.Nombre ?? string.Empty) : new BarrioDto(0, string.Empty),
        p.CodigoC != null ? new CodigoCDto(p.CodigoC.Id, p.CodigoC.Nombre ?? string.Empty) : new CodigoCDto(0, string.Empty),
        p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua.Id, l.Lengua.Nombre ?? string.Empty)).ToList() : new List<LenguaDto>(),
        p.LiderId,
        p.MesaVotacion != null ?
          new MesaVotacionDto(
            p.MesaVotacion.Id,
            p.MesaVotacion.Nombre ?? string.Empty,
            p.MesaVotacion.PuestoVotacion != null ?
              new PuestoVotacionDto(
                p.MesaVotacion.PuestoVotacion.Id,
                p.MesaVotacion.PuestoVotacion.Nombre ?? string.Empty
              ) :
              new PuestoVotacionDto(0, string.Empty)
          ) :
          new MesaVotacionDto(0, string.Empty, new PuestoVotacionDto(0, string.Empty)),
        p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB.Id, cb.CodigoB.Nombre ?? string.Empty)).ToList() : new List<CodigoBDto>(),
        p.PersonasACargo != null ? p.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>()
      ))
      .ToListAsync(cancellationToken);


    // Si se filtra por LiderId (con o sin CoordinadorId, pero sin checks de lideres/coordinadores true), agregar al propio líder si no está en la lista
    if (request.LiderId.HasValue && !(request.Lideres ?? false) && !(request.Coordinadores ?? false))
    {
      int liderId = request.LiderId.Value;
      bool yaIncluido = personas.Any(p => p.Id == liderId);
      if (!yaIncluido)
      {
        var lider = await db.Personas
          .AsNoTracking()
          .Include(p => p.Barrio)
          .Include(p => p.CodigoC)
          .Include(p => p.Lenguas!).ThenInclude(l => l.Lengua)
          .Include(p => p.MesaVotacion!).ThenInclude(mv => mv.PuestoVotacion)
          .Include(p => p.CodigosB!).ThenInclude(cb => cb.CodigoB)
          .Include(p => p.PersonasACargo)
          .FirstOrDefaultAsync(p => p.Id == liderId, cancellationToken);
        if (lider != null)
        {
          var liderDto = new PersonaListDto(
            lider.Id,
            lider.Nombre,
            lider.Apellido,
            lider.Cedula,
            lider.Apodo,
            lider.Telefono ?? string.Empty,
            lider.Direccion ?? string.Empty,
            lider.Descripcion ?? string.Empty,
            lider.IsLider,
            lider.IsCoordinador,
            lider.Barrio != null ? new BarrioDto(lider.Barrio.Id, lider.Barrio.Nombre ?? string.Empty) : new BarrioDto(0, string.Empty),
            lider.CodigoC != null ? new CodigoCDto(lider.CodigoC.Id, lider.CodigoC.Nombre ?? string.Empty) : new CodigoCDto(0, string.Empty),
            lider.Lenguas != null ? lider.Lenguas.Where(l => l.Lengua != null).Select(l => new LenguaDto(l.Lengua.Id, l.Lengua.Nombre ?? string.Empty)).ToList() : new List<LenguaDto>(),
            lider.LiderId,
            lider.MesaVotacion != null ?
              new MesaVotacionDto(
                lider.MesaVotacion.Id,
                lider.MesaVotacion.Nombre ?? string.Empty,
                lider.MesaVotacion.PuestoVotacion != null ?
                  new PuestoVotacionDto(
                    lider.MesaVotacion.PuestoVotacion.Id,
                    lider.MesaVotacion.PuestoVotacion.Nombre ?? string.Empty
                  ) :
                  new PuestoVotacionDto(0, string.Empty)
              ) :
              new MesaVotacionDto(0, string.Empty, new PuestoVotacionDto(0, string.Empty)),
            lider.CodigosB != null ? lider.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => new CodigoBDto(cb.CodigoB.Id, cb.CodigoB.Nombre ?? string.Empty)).ToList() : new List<CodigoBDto>(),
            lider.PersonasACargo != null ? lider.PersonasACargo.Select(pa => pa.Id).ToList() : new List<int>()
          );
          personas.Insert(0, liderDto);
        }
      }
    }

    return Result<List<PersonaListDto>>.Ok(personas);
  }
}
