using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Votacion.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record DeletePuestoCommand(int PuestoVotacionId) : IRequest<Result<DeletePuestoResponse>>;

public sealed record DeletePuestoResponse(string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class DeletePuestoCommandHandler(AppDbContext db) : IRequestHandler<DeletePuestoCommand, Result<DeletePuestoResponse>>
{
  public async Task<Result<DeletePuestoResponse>> Handle(DeletePuestoCommand request, CancellationToken cancellationToken)
  {
    var puesto = await db.PuestosVotacion
        .Include(p => p.MesasVotacion)
        .FirstOrDefaultAsync(p => p.Id == request.PuestoVotacionId, cancellationToken);
    if (puesto == null)
    {
      return Result<DeletePuestoResponse>.Fail(Error.NotFound("Puesto de votación no encontrado.", "PuestoVotacion.Delete.NotFound"));
    }

    var mesasIds = puesto.MesasVotacion.Select(m => m.Id).ToList();
    var existePersona = await db.Personas.AnyAsync(p => p.MesaVotacionId.HasValue && mesasIds.Contains(p.MesaVotacionId.Value), cancellationToken);
    if (existePersona)
    {
      return Result<DeletePuestoResponse>.Fail(Error.Conflict("No se puede eliminar el puesto porque alguna de sus mesas tiene personas asociadas.", "PuestoVotacion.Delete.MesasConPersonas"));
    }

    db.PuestosVotacion.Remove(puesto);
    await db.SaveChangesAsync(cancellationToken);
    return Result<DeletePuestoResponse>.Ok(new DeletePuestoResponse("Puesto de votación eliminado exitosamente."));
  }
}
