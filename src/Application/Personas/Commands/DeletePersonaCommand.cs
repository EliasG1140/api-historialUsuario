using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Personas.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record DeletePersonaCommand(int Id) : IRequest<Result<DeletePersonaResponse>>;

public sealed record DeletePersonaResponse(string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class DeletePersonaCommandHandler(AppDbContext db) : IRequestHandler<DeletePersonaCommand, Result<DeletePersonaResponse>>
{
  public async Task<Result<DeletePersonaResponse>> Handle(DeletePersonaCommand request, CancellationToken cancellationToken)
  {
    var persona = await db.Personas
        .Include(p => p.CodigosB)
        .Include(p => p.PersonasACargo)
        .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
    if (persona == null)
    {
      return Result<DeletePersonaResponse>.Fail(Error.NotFound("Persona no encontrada.", "Persona.Delete.NotFound"));
    }

    if (persona.IsLider && persona.PersonasACargo.Any())
    {
      return Result<DeletePersonaResponse>.Fail(Error.Conflict("No se puede eliminar un líder que tiene personas a cargo.", "Persona.Delete.LiderConPersonasACargo"));
    }

    db.Personas.Remove(persona);
    await db.SaveChangesAsync(cancellationToken);
    return Result<DeletePersonaResponse>.Ok(new DeletePersonaResponse("Persona eliminada exitosamente."));
  }
}
