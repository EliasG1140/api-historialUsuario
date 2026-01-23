using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Application.Personas.Queries;

//* ---------------------------------- Query --------------------------------- */
public sealed record ExportCoordinadoresToExcelQuery() : IRequest<Result<ExcelFileDto>>;

//* --------------------------------- Handler -------------------------------- */
public sealed class ExportCoordinadoresToExcelQueryHandler(AppDbContext db)
  : IRequestHandler<ExportCoordinadoresToExcelQuery, Result<ExcelFileDto>>
{
  public async Task<Result<ExcelFileDto>> Handle(ExportCoordinadoresToExcelQuery q, CancellationToken ct)
  {
    var rows = await db.Personas
      .AsNoTracking()
      .Include(p => p.MesaVotacion).ThenInclude(m => m.PuestoVotacion)
      .Where(p => p.IsCoordinador)
      .OrderBy(p => p.Apellido)
      .ThenBy(p => p.Nombre)
      .Select(p => new
      {
        p.Nombre,
        p.Apellido,
        NumeroDoc = p.Cedula,
        Puesto = p.MesaVotacion.PuestoVotacion.Nombre,
        CantidadLideresACargo = p.Coordinados.Count
      })
      .ToListAsync(ct);

    // Build Excel
    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("Coordinadores");

    // Header
    var headers = new[]
    {
      "Nombre", "Apellido", "Numero de identificacion", "Puesto de votacion", "Cantidad de lideres a cargo"
    };

    for (var i = 0; i < headers.Length; i++)
      ws.Cell(1, i + 1).Value = headers[i];

    ws.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);
    ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

    // Body
    var r = 2;
    foreach (var x in rows)
    {
      ws.Cell(r, 1).Value = x.Nombre;
      ws.Cell(r, 2).Value = x.Apellido;
      ws.Cell(r, 3).Value = x.NumeroDoc;
      ws.Cell(r, 4).Value = x.Puesto;
      ws.Cell(r, 5).Value = x.CantidadLideresACargo;
      r++;
    }

    ws.Columns().AdjustToContents();

    using var ms = new MemoryStream();
    wb.SaveAs(ms);

    var fileName = $"coordinadores_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

    return Result<ExcelFileDto>.Ok(new ExcelFileDto(
      ms.ToArray(),
      fileName,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ));
  }
}
