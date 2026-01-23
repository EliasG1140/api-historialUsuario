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
  public async Task<IActionResult> ExportPersonas(
    [FromQuery] int? lider,
    [FromQuery] int? coordinador,
    [FromQuery] bool? lideres,
    [FromQuery] bool? coordinadores,
    [FromQuery] int? puestoVotacion,
    [FromQuery] int? mesaVotacion,
    [FromQuery] int? codigoB,
    [FromQuery] int? codigoC,
    [FromQuery] int? categoria,
    CancellationToken ct)
  {
    var query = new ExportPersonasToExcelQuery(
      Lider: lider,
      Coordinador: coordinador,
      Lideres: lideres,
      Coordinadores: coordinadores,
      PuestoVotacion: puestoVotacion,
      MesaVotacion: mesaVotacion,
      CodigoB: codigoB,
      CodigoC: codigoC,
      Categoria: categoria
    );
    var result = await _mediator.Send(query, ct);
    if (!result.Succeeded)
      return result.ToActionResult(this);

    var file = result.Value!;
    return File(file.Content, file.ContentType, file.FileName);
  }

  [HttpGet("lideres/excel")]
  public async Task<IActionResult> ExportLideres(CancellationToken ct)
  {

    var result = await _mediator.Send(new ExportLideresToExcelQuery(), ct);
    if (!result.Succeeded)
      return result.ToActionResult(this);

    var file = result.Value!;
    return File(file.Content, file.ContentType, file.FileName);
  }

  [HttpGet("coordinadores/excel")]
  public async Task<IActionResult> ExportCoordinadores(CancellationToken ct)
  {

    var result = await _mediator.Send(new ExportCoordinadoresToExcelQuery(), ct);
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
    [FromQuery] int? coordinador,
    [FromQuery] bool? lideres,
    [FromQuery] bool? coordinadores,
    [FromQuery] int? puestoVotacion,
    [FromQuery] int? mesaVotacion,
    [FromQuery] int? codigoB,
    [FromQuery] int? codigoC,
    [FromQuery] int? categoria,
    CancellationToken ct)
  {
    var query = new GetPersonasQuery(
      LiderId: lider,
      CoordinadorId: coordinador,
      Lideres: lideres,
      Coordinadores: coordinadores,
      PuestoVotacionId: puestoVotacion,
      MesaVotacionId: mesaVotacion,
      CodigoBId: codigoB,
      CodigoCId: codigoC,
      CategoriaId: categoria
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

  [HttpGet("coordinadores")]
  public async Task<IActionResult> GetCoordinadores(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetCoordinadoresQuery(), ct);
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
