using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Application.Personas.Queries;

//* ---------------------------------- Query --------------------------------- */
public sealed record ExportPersonasToExcelQuery(
  int? Lider,
  int? Coordinador,
  bool? Lideres,
  bool? Coordinadores,
  int? PuestoVotacion,
  int? MesaVotacion,
  int? CodigoB,
  int? CodigoC,
  int? Categoria
) : IRequest<Result<ExcelFileDto>>;

public sealed record ExcelFileDto(
  byte[] Content,
  string FileName,
  string ContentType
);

//* --------------------------------- Handler -------------------------------- */
public sealed class ExportPersonasToExcelQueryHandler(AppDbContext db)
  : IRequestHandler<ExportPersonasToExcelQuery, Result<ExcelFileDto>>
{
  public async Task<Result<ExcelFileDto>> Handle(ExportPersonasToExcelQuery q, CancellationToken ct)
  {



    var query = db.Personas
      .AsNoTracking()
      .AsSplitQuery()
      .Include(p => p.Barrio)
      .Include(p => p.MesaVotacion).ThenInclude(m => m.PuestoVotacion)
      .Include(p => p.Lenguas!).ThenInclude(pl => pl.Lengua)
      .AsQueryable();

    // Filtro exclusivo para coordinadores
    if (q.Coordinadores.HasValue && q.Coordinadores.Value)
    {
      query = query.Where(p => p.IsCoordinador);
    }
    // Filtro exclusivo para lideres
    else if (q.Lideres.HasValue && q.Lideres.Value)
    {
      query = query.Where(p => p.IsLider);
      if (q.Coordinador.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == q.Coordinador.Value);
      }
      if (q.Categoria.HasValue)
      {
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == q.Categoria.Value, ct);
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
      if (q.Coordinador.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == q.Coordinador.Value);
      }
      // Si se selecciona un LiderId, trae todas las personas con ese LiderId y también incluye al propio líder
      else if (q.Lider.HasValue)
      {
        int liderId = q.Lider.Value;
        query = query.Where(p => p.LiderId == liderId || p.Id == liderId);
      }
      // Si no hay filtros, trae todas las personas (no se aplica filtro)
    }


    // Filtro por coordinadores
    if (q.Coordinadores.HasValue && q.Coordinadores.Value)
    {
      query = query.Where(p => p.IsCoordinador);
    }
    // Filtro por lideres (tiene prioridad sobre lider individual)
    else if (q.Lideres.HasValue && q.Lideres.Value)
    {
      query = query.Where(p => p.IsLider);
      // Si se especifica CoordinadorId, filtrar los líderes bajo ese coordinador
      if (q.Coordinador.HasValue)
      {
        query = query.Where(p => p.CoordinadorId == q.Coordinador.Value);
      }
      // Lógica de categoría solo si Lideres == true y CategoriaId tiene valor
      if (q.Categoria.HasValue)
      {
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == q.Categoria.Value, ct);
        if (categoria != null)
        {
          query = query.Where(p => p.PersonasACargo.Count >= categoria.Minimo && p.PersonasACargo.Count <= categoria.Maximo);
        }
      }
    }
    else if (q.Lider.HasValue)
    {
      query = query.Where(p => p.LiderId == q.Lider.Value);
    }

    // Filtros adicionales (AND)
    if (q.PuestoVotacion.HasValue)
    {
      query = query.Where(p => p.MesaVotacion != null && p.MesaVotacion.PuestoVotacionId == q.PuestoVotacion.Value);
    }
    if (q.MesaVotacion.HasValue)
    {
      query = query.Where(p => p.MesaVotacionId == q.MesaVotacion.Value);
    }
    if (q.CodigoB.HasValue)
    {
      query = query.Where(p => p.CodigosB != null && p.CodigosB.Any(cb => cb.CodigoBId == q.CodigoB.Value));
    }
    if (q.CodigoC.HasValue)
    {
      query = query.Where(p => p.CodigoCId == q.CodigoC.Value);
    }

    var rowsList = await query
      .Include(p => p.CodigosB!).ThenInclude(pb => pb.CodigoB)
      .Include(p => p.CodigoC)
      .Include(p => p.Lider)
      .Include(p => p.Coordinador)
      .OrderBy(p => p.Apellido)
      .ThenBy(p => p.Nombre)
      .Select(p => new
      {
        Coordinador = p.Coordinador != null ? p.Coordinador.Nombre + " " + p.Coordinador.Apellido : string.Empty,
        Lider = p.Lider != null ? p.Lider.Nombre + " " + p.Lider.Apellido : string.Empty,
        p.Nombre,
        p.Apellido,
        NumeroDoc = p.Cedula,
        p.Apodo,
        p.Familia,
        p.Telefono,
        Barrio = p.Barrio != null ? p.Barrio.Nombre : string.Empty,
        p.Direccion,
        Lenguas = p.Lenguas != null ? p.Lenguas.Where(l => l.Lengua != null).Select(l => l.Lengua.Nombre) : new List<string>(),
        Puesto = p.MesaVotacion != null && p.MesaVotacion.PuestoVotacion != null ? p.MesaVotacion.PuestoVotacion.Nombre : string.Empty,
        Mesa = p.MesaVotacion != null ? p.MesaVotacion.Nombre : string.Empty,
        CodigosC = p.CodigoC != null ? p.CodigoC.Nombre : string.Empty,
        CodigosB = p.CodigosB != null ? p.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => cb.CodigoB.Nombre) : new List<string>(),
        Descripcion = p.Descripcion ?? string.Empty,
        IsLider = p.IsLider,
        IsCoordinador = p.IsCoordinador,
        PersonasACargo = p.PersonasACargo.Count
      })
      .ToListAsync(ct);

    // Si se filtra por Lider (con o sin Coordinador, pero sin checks de lideres/coordinadores true), agregar al propio líder si no está en la lista
    if (q.Lider.HasValue && !(q.Lideres ?? false) && !(q.Coordinadores ?? false))
    {
      int liderId = q.Lider.Value;
      bool yaIncluido = rowsList.Any(p => db.Personas.Any(x => x.Id == liderId && x.Cedula == p.NumeroDoc));
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
          .Include(p => p.Lider)
          .Include(p => p.Coordinador)
          .FirstOrDefaultAsync(p => p.Id == liderId, ct);
        if (lider != null)
        {
          var liderRow = new
          {
            Coordinador = lider.Coordinador != null ? lider.Coordinador.Nombre + " " + lider.Coordinador.Apellido : string.Empty,
            Lider = lider.Lider != null ? lider.Lider.Nombre + " " + lider.Lider.Apellido : string.Empty,
            Nombre = lider.Nombre,
            Apellido = lider.Apellido,
            NumeroDoc = lider.Cedula,
            Apodo = lider.Apodo,
            Familia = lider.Familia,
            Telefono = lider.Telefono,
            Barrio = lider.Barrio != null ? lider.Barrio.Nombre : string.Empty,
            Direccion = lider.Direccion,
            Lenguas = lider.Lenguas != null ? lider.Lenguas.Where(l => l.Lengua != null).Select(l => l.Lengua.Nombre) : new List<string>(),
            Puesto = lider.MesaVotacion != null && lider.MesaVotacion.PuestoVotacion != null ? lider.MesaVotacion.PuestoVotacion.Nombre : string.Empty,
            Mesa = lider.MesaVotacion != null ? lider.MesaVotacion.Nombre : string.Empty,
            CodigosC = lider.CodigoC != null ? lider.CodigoC.Nombre : string.Empty,
            CodigosB = lider.CodigosB != null ? lider.CodigosB.Where(cb => cb.CodigoB != null).Select(cb => cb.CodigoB.Nombre) : new List<string>(),
            Descripcion = lider.Descripcion ?? string.Empty,
            IsLider = lider.IsLider,
            IsCoordinador = lider.IsCoordinador,
            PersonasACargo = lider.PersonasACargo.Count
          };
          rowsList.Insert(0, liderRow);
        }
      }
    }


    var rows = rowsList;

    // 2) Build Excel
    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("Personas");

    // Header
    var headers = new List<string>
    {
      "Coordinador", "Lider", "Nombre", "Apellido", "Numero Doc", "Apodo", "Familia", "Telefono", "Barrio", "Direccion", "Lenguas", "Puesto de votación", "Mesa", "Codigos C", "Codigos B", "Descripcion", "Es Líder", "Es Coordinador", "Personas a Cargo"
    };

    for (var i = 0; i < headers.Count; i++)
      ws.Cell(1, i + 1).Value = headers[i];

    ws.Range(1, 1, 1, headers.Count).Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);
    ws.Range(1, 1, 1, headers.Count).SetAutoFilter();

    // Body
    var r = 2;
    foreach (var x in rows)
    {
      ws.Cell(r, 1).Value = x.Coordinador;
      ws.Cell(r, 2).Value = x.Lider;
      ws.Cell(r, 3).Value = x.Nombre;
      ws.Cell(r, 4).Value = x.Apellido;
      ws.Cell(r, 5).Value = x.NumeroDoc;
      ws.Cell(r, 6).Value = x.Apodo;
      ws.Cell(r, 7).Value = x.Familia;
      ws.Cell(r, 8).Value = x.Telefono;
      ws.Cell(r, 9).Value = x.Barrio ?? string.Empty;
      ws.Cell(r, 10).Value = x.Direccion ?? string.Empty;
      ws.Cell(r, 11).Value = x.Lenguas != null ? string.Join(", ", x.Lenguas) : string.Empty;
      ws.Cell(r, 12).Value = x.Puesto ?? string.Empty;
      ws.Cell(r, 13).Value = x.Mesa ?? string.Empty;
      ws.Cell(r, 14).Value = x.CodigosC ?? string.Empty;
      ws.Cell(r, 15).Value = x.CodigosB != null ? string.Join(", ", x.CodigosB) : string.Empty;
      ws.Cell(r, 16).Value = x.Descripcion ?? string.Empty;
      ws.Cell(r, 17).Value = x.IsLider;
      ws.Cell(r, 18).Value = x.IsCoordinador;
      ws.Cell(r, 19).Value = x.PersonasACargo;
      r++;
    }

    ws.Columns().AdjustToContents();

    using var ms = new MemoryStream();
    wb.SaveAs(ms);

    var fileName = $"personas_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

    return Result<ExcelFileDto>.Ok(new ExcelFileDto(
      ms.ToArray(),
      fileName,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ));
  }
}