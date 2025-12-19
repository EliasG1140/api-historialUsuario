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
  int? MesaVotacion
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
      query = query.Where(p => p.IsLider);

    if (q.PuestoVotacion is not null)
      query = query.Where(p => p.MesaVotacion.PuestoVotacion.Id == q.PuestoVotacion);

    if (q.MesaVotacion is not null)
      query = query.Where(p => p.MesaVotacion.Id == q.MesaVotacion);

    var rows = await query
      .OrderBy(p => p.Id)
      .Select(p => new PersonaExcelRow(
        p.Id,
        p.Nombre,
        p.Apellido,
        p.Cedula,
        p.Telefono,
        p.Direccion,
        p.IsLider,
        p.Barrio.Nombre,
        p.MesaVotacion.Nombre,
        p.MesaVotacion.PuestoVotacion.Nombre
      ))
      .ToListAsync(ct);

    // 2) Build Excel
    using var wb = new XLWorkbook();
    var ws = wb.Worksheets.Add("Personas");

    // Header
    var headers = new[]
    {
      "Id","Nombre","Apellido","Cédula","Teléfono","Dirección","Barrio","Puesto","Mesa","¿Líder?"
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
      ws.Cell(r, 1).Value = x.Id;
      ws.Cell(r, 2).Value = x.Nombre;
      ws.Cell(r, 3).Value = x.Apellido;
      ws.Cell(r, 4).Value = x.Cedula;
      ws.Cell(r, 5).Value = x.Telefono;
      ws.Cell(r, 6).Value = x.Barrio;
      ws.Cell(r, 7).Value = x.Direccion;
      ws.Cell(r, 8).Value = x.Puesto;
      ws.Cell(r, 9).Value = x.Mesa;
      ws.Cell(r, 10).Value = x.IsLider ? "Sí" : "No";
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