using Application.Home.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/home")]
public class HomeController(IMediator mediator) : ControllerBase
{
  private readonly IMediator _mediator = mediator;

  /* ----------------------------------- Get ---------------------------------- */
  [HttpGet("categorias")]
  public async Task<IActionResult> GetCategorias(CancellationToken ct)
  {
    var result = await _mediator.Send(new GetCategorizacionQuery(), ct);
    return result.ToActionResult(this);
  }
}