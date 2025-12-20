using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Application.Personas.Queries;

//* ---------------------------------- Query --------------------------------- */
public sealed record ExportPersonasToExcelQuery(
  int? Lider,
  bool? Lideres,
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
      .Include(p => p.Barrio)
      .Include(p => p.MesaVotacion).ThenInclude(m => m.PuestoVotacion)
      .AsQueryable();

    if (q.Lider is not null)
      query = query.Where(p => p.LiderId == q.Lider);

    if (q.Lideres is true)
    {
      query = query.Where(p => p.IsLider);

      // Lógica de categoría solo si Lideres == true y Categoria tiene valor
      if (q.Categoria.HasValue)
      {
        var categoria = await db.Categorias.FirstOrDefaultAsync(c => c.Id == q.Categoria.Value, ct);
        if (categoria != null)
        {
          query = query.Where(p => p.PersonasACargo.Count >= categoria.Minimo && p.PersonasACargo.Count <= categoria.Maximo);
        }
      }
    }

    if (q.PuestoVotacion is not null)
      query = query.Where(p => p.MesaVotacion.PuestoVotacion.Id == q.PuestoVotacion);

    if (q.MesaVotacion is not null)
      query = query.Where(p => p.MesaVotacion.Id == q.MesaVotacion);

    if (q.CodigoB is not null)
      query = query.Where(p => p.CodigosB!.Any(cb => cb.CodigoBId == q.CodigoB));

    if (q.CodigoC is not null)
      query = query.Where(p => p.CodigoCId == q.CodigoC);

    var rows = await query
      .Include(p => p.Lenguas).ThenInclude(pl => pl.Lengua)
      .Include(p => p.CodigosB!).ThenInclude(pb => pb.CodigoB)
      .Include(p => p.CodigoC)
      .OrderBy(p => p.Apellido)
      .ThenBy(p => p.Nombre)
      .Select(p => new
      {
        p.Nombre,
        p.Apellido,
        NumeroDoc = p.Cedula,
        p.Apodo,
        p.Telefono,
        Barrio = p.Barrio.Nombre,
        p.Direccion,
        Lenguas = p.Lenguas.Select(l => l.Lengua.Nombre),
        Puesto = p.MesaVotacion.PuestoVotacion.Nombre,
        Mesa = p.MesaVotacion.Nombre,
        CodigosC = p.CodigoC.Nombre,
        CodigosB = p.CodigosB!.Select(cb => cb.CodigoB.Nombre),
        p.Descripcion,
        PersonasACargo = q.Lideres == true ? p.PersonasACargo.Count : (int?)null
      })
      .ToListAsync(ct);

    // 2) Build Excel
    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("Personas");

    // Header
    var headers = new List<string>
    {
      "Nombre", "Apellido", "Numero Doc", "Apodo", "Telefono", "Barrio", "Direccion", "Lenguas", "Puesto de votación", "Mesa", "Codigos C", "Codigos B", "Descripcion"
    };
    if (q.Lideres == true)
      headers.Add("Personas");

    for (var i = 0; i < headers.Count; i++)
      ws.Cell(1, i + 1).Value = headers[i];

    ws.Range(1, 1, 1, headers.Count).Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);
    ws.Range(1, 1, 1, headers.Count).SetAutoFilter();

    // Body
    var r = 2;
    foreach (var x in rows)
    {
      ws.Cell(r, 1).Value = x.Nombre;
      ws.Cell(r, 2).Value = x.Apellido;
      ws.Cell(r, 3).Value = x.NumeroDoc;
      ws.Cell(r, 4).Value = x.Apodo;
      ws.Cell(r, 5).Value = x.Telefono;
      ws.Cell(r, 6).Value = x.Barrio;
      ws.Cell(r, 7).Value = x.Direccion;
      ws.Cell(r, 8).Value = string.Join(", ", x.Lenguas);
      ws.Cell(r, 9).Value = x.Puesto;
      ws.Cell(r, 10).Value = x.Mesa;
      ws.Cell(r, 11).Value = x.CodigosC;
      ws.Cell(r, 12).Value = string.Join(", ", x.CodigosB);
      ws.Cell(r, 13).Value = x.Descripcion;
      if (q.Lideres == true)
        ws.Cell(r, 14).Value = x.PersonasACargo;
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