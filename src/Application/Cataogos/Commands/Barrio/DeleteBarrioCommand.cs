using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Barrio;

//* --------------------------------- Command -------------------------------- */
public sealed record DeleteBarrioCommand(int Id) : IRequest<Result<DeleteBarrioResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record DeleteBarrioResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class DeleteBarrioCommandHandler(AppDbContext db) : IRequestHandler<DeleteBarrioCommand, Result<DeleteBarrioResponse>>
{
  public async Task<Result<DeleteBarrioResponse>> Handle(DeleteBarrioCommand request, CancellationToken cancellationToken)
  {
    var barrio = await db.Barrios.Include(b => b.Personas).FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
    if (barrio is null)
    {
      return Result<DeleteBarrioResponse>.Fail(Error.NotFound("Barrio no encontrado.", "Barrio.Delete.NotFound"));
    }

    int personasCount = barrio.Personas?.Count ?? 0;
    if (personasCount > 0)
    {
      return Result<DeleteBarrioResponse>.Fail(Error.Conflict($"No se puede borrar el barrio porque hay {personasCount} personas registradas.", "Barrio.Delete.PersonasExist"));
    }

    db.Barrios.Remove(barrio);
    await db.SaveChangesAsync(cancellationToken);

    return Result<DeleteBarrioResponse>.Ok(new DeleteBarrioResponse("Barrio eliminado exitosamente."));
  }
}
