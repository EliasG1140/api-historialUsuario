using Application.Personas.Commands;
using Application.Personas.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Authorize]
[ApiController]
[Route("api/personas")]
public class PersonaController(IMediator mediator) : ControllerBase
{
  private readonly IMediator _mediator = mediator;

  /* ----------------------------------- Get ---------------------------------- */
  [HttpGet("{id:int}")]
  public async Task<IActionResult> GetPersonaById(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new GetPersonaByIdQuery(id), ct);
    return result.ToActionResult(this);
  }

  /* ---------------------------------- Post ---------------------------------- */
  [HttpPost]
  public async Task<IActionResult> Create([FromBody] CreatePersonaCommand command, CancellationToken ct)
  {
    var isAuth = User?.Identity?.IsAuthenticated;
    var claims = User?.Claims.Select(c => new { c.Type, c.Value }).ToList();

    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpGet("excel")]
  public async Task<IActionResult> ImportFromExcel(
    [FromQuery] int? lider,
    [FromQuery] bool? lideres,
    [FromQuery] int? puestoVotacion,
    [FromQuery] int? mesaVotacion,
    CancellationToken ct)
  {
    var query = new ExportPersonasToExcelQuery(
      Lider: lider,
      Lideres: lideres,
      PuestoVotacion: puestoVotacion,
      MesaVotacion: mesaVotacion
    );
    var result = await _mediator.Send(query, ct);
    if (!result.Succeeded)
      return result.ToActionResult(this);

    var file = result.Value!;
    return File(file.Content, file.ContentType, file.FileName);
  }

  /* ----------------------------------- Put ---------------------------------- */
  // GET /api/personas?lider={idLider}&lideres={true|false}&puestoVotacion={idPuesto}&mesaVotacion={idMesa}
  [HttpGet]
  public async Task<IActionResult> GetPersonas(
    [FromQuery] int? lider,
    [FromQuery] bool? lideres,
    [FromQuery] int? puestoVotacion,
    [FromQuery] int? mesaVotacion,
    CancellationToken ct)
  {
    var query = new GetPersonasQuery(
      LiderId: lider,
      Lideres: lideres,
      PuestoVotacionId: puestoVotacion,
      MesaVotacionId: mesaVotacion
    );
    var result = await _mediator.Send(query, ct);
    return result.ToActionResult(this);
  }

  [HttpGet("lideres")]
  public async Task<IActionResult> GetLideres(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetLideresQuery(), ct);
    return result.ToActionResult(this);
  }



  [HttpPut("{id:int}")]
  public async Task<IActionResult> Update(int id, [FromBody] UpdatePersonaCommand command, CancellationToken ct)
  {
    command = command with { Id = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  /* ------------------------------- Delete ------------------------------- */
  [HttpDelete("{id:int}")]
  public async Task<IActionResult> Delete(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeletePersonaCommand(id), ct);
    return result.ToActionResult(this);
  }
}
