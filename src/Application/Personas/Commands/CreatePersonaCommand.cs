using Infrastructure.Data;
using MediatR;
using Domain.Personas;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Personas.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record CreatePersonaCommand(
  string Nombre,
  string Apellido,
  string Cedula,
  string? Apodo,
  string Telefono,
  string? Direccion,
  string? Descripcion,
  bool EsLider,
  int? Barrio,
  int CodigoC,
  List<int>? Lengua,
  int? Lider,
  int MesaVotacion,
  List<int>? CodigoB
) : IRequest<Result<CreatePersonaResponse>>;

public sealed record CreatePersonaResponse(int Id, string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class CreatePersonaCommandHandler : IRequestHandler<CreatePersonaCommand, Result<CreatePersonaResponse>>
{
  private readonly AppDbContext db;
  private readonly IHttpContextAccessor httpContextAccessor;

  public CreatePersonaCommandHandler(AppDbContext db, IHttpContextAccessor httpContextAccessor)
  {
    this.db = db;
    this.httpContextAccessor = httpContextAccessor;
  }

  public async Task<Result<CreatePersonaResponse>> Handle(CreatePersonaCommand request, CancellationToken cancellationToken)
  {
    var existsCedula = await db.Personas.AnyAsync(p => p.Cedula == request.Cedula, cancellationToken);
    if (existsCedula)
    {
      return Result<CreatePersonaResponse>.Fail(Error.Conflict("La cédula ya está registrada.", "Persona.Create.CedulaExists"));
    }

    var user = httpContextAccessor.HttpContext?.User;
    string? userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value
      ?? user?.FindFirst("sub")?.Value;

    var persona = new Persona
    {
      Nombre = request.Nombre.ToUpper(),
      Apellido = request.Apellido.ToUpper(),
      Cedula = request.Cedula,
      Apodo = request.Apodo != null ? request.Apodo.ToUpper() : null,
      Telefono = request.Telefono,
      Direccion = request.Direccion ?? null,
      Descripcion = request.Descripcion ?? null,
      IsLider = request.EsLider,
      BarrioId = request.Barrio ?? null,
      CodigoCId = request.CodigoC,
      LiderId = request.Lider,
      MesaVotacionId = request.MesaVotacion,
      CreatedAt = DateTime.UtcNow,
      CreatedByUserId = userId,
      LastModifiedAt = null,
      LastModifiedByUserId = null
    };

    if (request.Lengua is { Count: > 0 })
    {
      persona.Lenguas = request.Lengua.Select(id => new PersonaLengua { LenguaId = id }).ToList();
    }

    if (request.CodigoB is { Count: > 0 })
    {
      persona.CodigosB = request.CodigoB.Select(id => new PersonaCodigoB { CodigoBId = id }).ToList();
    }

    db.Personas.Add(persona);
    await db.SaveChangesAsync(cancellationToken);
    return Result<CreatePersonaResponse>.Ok(new CreatePersonaResponse(persona.Id, "Persona registrada exitosamente."));
  }
}