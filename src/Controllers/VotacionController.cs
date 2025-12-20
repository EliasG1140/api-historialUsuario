using Application.Votacion.Commands;
using Application.Votacion.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/votacion")]
public class VotacionController(IMediator mediator) : ControllerBase
{
  private readonly IMediator _mediator = mediator;
  /* ------------------------------- Get ------------------------------- */
  [HttpGet("puestos")]
  public async Task<IActionResult> GetPuestos(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetPuestosQuery(), ct);
    return Ok(result);
  }

  [HttpGet("puestos/excel")]
  public async Task<IActionResult> ExportPuestosVotacionToExcel(CancellationToken ct)
  {
    var result = await _mediator.Send(new ExportPuestosVotacionToExcelQuery(), ct);
    if (!result.Succeeded || result.Value == null)
      return BadRequest(result.Error);
    var file = result.Value;
    return File(file.Content, file.ContentType, file.FileName);
  }

  [HttpGet("puestos/consulta")]
  public async Task<IActionResult> GetPuestosConsulta(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetPuestosConsultaQuery(), ct);
    return Ok(result);
  }

  [HttpGet("puestos/{puestoId:int}/mesas")]
  public async Task<IActionResult> GetMesasByPuestoId(int puestoId, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetMesasByPuestoIdQuery(puestoId), ct);
    return Ok(result);
  }

  /* ------------------------------- Post ------------------------------- */
  [HttpPost("puestos")]
  public async Task<IActionResult> CreatePuesto([FromBody] CreatePuestoCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  /* ------------------------------- Put -------------------------------- */
  [HttpPut("puestos/{id:int}")]
  public async Task<IActionResult> UpdatePuesto(int id, [FromBody] UpdatePuestoCommand command, CancellationToken ct)
  {
    command = command with { PuestoVotacionId = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  /* ------------------------------- Delete ------------------------------- */
  [HttpDelete("puestos/{id:int}")]
  public async Task<IActionResult> DeletePuesto(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeletePuestoCommand(id), ct);
    return result.ToActionResult(this);
  }
}
