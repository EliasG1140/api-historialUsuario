using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;

namespace Application.Votacion.Queries;

public sealed record ExportPuestosVotacionToExcelQuery() : IRequest<Result<ExcelFileDto>>;

public sealed record ExcelFileDto(
  byte[] Content,
  string FileName,
  string ContentType
);

public sealed class ExportPuestosVotacionToExcelQueryHandler(AppDbContext db)
  : IRequestHandler<ExportPuestosVotacionToExcelQuery, Result<ExcelFileDto>>
{
  public async Task<Result<ExcelFileDto>> Handle(ExportPuestosVotacionToExcelQuery q, CancellationToken ct)
  {
    var puestos = await db.PuestosVotacion
      .Include(p => p.MesasVotacion)
      .ThenInclude(m => m.Personas)
      .OrderBy(p => p.Nombre)
      .ToListAsync(ct);

    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("PuestosVotacion");

    var headers = new[]
    {
      "Puesto de votacion", "Cantidad de mesas", "No de mesa", "Personas"
    };
    for (var i = 0; i < headers.Length; i++)
      ws.Cell(1, i + 1).Value = headers[i];
    ws.Range(1, 1, 1, headers.Length).Style.Font.Bold = true;
    ws.SheetView.FreezeRows(1);
    ws.Range(1, 1, 1, headers.Length).SetAutoFilter();

    int r = 2;
    foreach (var puesto in puestos)
    {
      // Fila principal del puesto
      ws.Cell(r, 1).Value = puesto.Nombre;
      ws.Cell(r, 2).Value = puesto.MesasVotacion.Count;
      ws.Cell(r, 3).Value = "";
      ws.Cell(r, 4).Value = puesto.MesasVotacion.SelectMany(m => m.Personas).Count();
      r++;
      // Filas de mesas
      foreach (var mesa in puesto.MesasVotacion.OrderBy(m => m.Nombre))
      {
        ws.Cell(r, 1).Value = puesto.Nombre;
        ws.Cell(r, 2).Value = "";
        ws.Cell(r, 3).Value = mesa.Nombre;
        ws.Cell(r, 4).Value = mesa.Personas.Count;
        r++;
      }
    }

    ws.Columns().AdjustToContents();
    using var ms = new MemoryStream();
    wb.SaveAs(ms);
    var fileName = $"puestos_votacion_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
    return Result<ExcelFileDto>.Ok(new ExcelFileDto(
      ms.ToArray(),
      fileName,
      "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    ));
  }
}
