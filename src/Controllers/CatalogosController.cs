using Application.Catalogos.Queries;
using Application.Cataogos.Commands.Barrio;
using Application.Cataogos.Commands.Categoria;
using Application.Cataogos.Commands.CodigoB;
using Application.Cataogos.Commands.CodigoC;
using Application.Cataogos.Commands.Lengua;
using Application.Cataogos.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/catalogos")]
public class CatalogoController(IMediator mediator) : ControllerBase
{
  private readonly IMediator _mediator = mediator;

  /* ----------------------------------- Get ---------------------------------- */
  [HttpGet("barrios")]
  public async Task<IActionResult> GetBarrios(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetBarriosQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpGet("codigosb")]
  public async Task<IActionResult> GetCodigosB(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetCodigoBsQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpGet("codigosc")]
  public async Task<IActionResult> GetCodigosC(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetCodigoCsQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpGet("lenguas")]
  public async Task<IActionResult> GetLenguas(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetLenguasQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpGet("categorias")]
  public async Task<IActionResult> GetCategorias(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetCategoriasQuery(), ct);
    return result.ToActionResult(this);
  }

  [HttpGet("lideres")]
  public async Task<IActionResult> GetLideres(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetPersonaLiderQuery(), ct);
    return result.ToActionResult(this);
  }

  /* ----------------------------------- Post ---------------------------------- */
  [HttpPost("barrios")]
  public async Task<IActionResult> CreateBarrio([FromBody] CreateBarrioCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPost("codigosb")]
  public async Task<IActionResult> CreateCodigoB([FromBody] CreateCodigoBCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPost("codigosc")]
  public async Task<IActionResult> CreateCodigoC([FromBody] CreateCodigoCCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPost("lenguas")]
  public async Task<IActionResult> CreateLengua([FromBody] CreateLenguaCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPost("categorias")]
  public async Task<IActionResult> CreateCategoria([FromBody] CreateCategoriaCommand command, CancellationToken ct)
  {
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  /* --------------------------------- Update --------------------------------- */
  [HttpPut("barrios/{id:int}")]
  public async Task<IActionResult> UpdateBarrio(int id, [FromBody] UpdateBarrioCommand command, CancellationToken ct)
  {
    command = command with { Id = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPut("codigosb/{id:int}")]
  public async Task<IActionResult> UpdateCodigoB(int id, [FromBody] UpdateCodigoBCommand command, CancellationToken ct)
  {
    command = command with { Id = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPut("codigosc/{id:int}")]
  public async Task<IActionResult> UpdateCodigoC(int id, [FromBody] UpdateCodigoCCommand command, CancellationToken ct)
  {
    command = command with { Id = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  [HttpPut("lenguas/{id:int}")]
  public async Task<IActionResult> UpdateLengua(int id, [FromBody] UpdateLenguaCommand command, CancellationToken ct)
  {
    command = command with { Id = id };
    var result = await _mediator.Send(command, ct);
    return result.ToActionResult(this);
  }

  /* --------------------------------- Delete --------------------------------- */
  [HttpDelete("barrios/{id:int}")]
  public async Task<IActionResult> DeleteBarrio(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteBarrioCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpDelete("codigosb/{id:int}")]
  public async Task<IActionResult> DeleteCodigoB(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteCodigoBCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpDelete("codigosc/{id:int}")]
  public async Task<IActionResult> DeleteCodigoC(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteCodigoCCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpDelete("lenguas/{id:int}")]
  public async Task<IActionResult> DeleteLengua(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteLenguaCommand(id), ct);
    return result.ToActionResult(this);
  }

  [HttpDelete("categorias/{id:int}")]
  public async Task<IActionResult> DeleteCategoria(int id, CancellationToken ct)
  {
    var result = await _mediator.Send(new DeleteCategoriaCommand(id), ct);
    return result.ToActionResult(this);
  }

}
