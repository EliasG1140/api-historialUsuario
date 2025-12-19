using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Barrio;

//* --------------------------------- Command -------------------------------- */
public sealed record UpdateBarrioCommand(int Id, string Nombre) : IRequest<Result<UpdateBarrioResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record UpdateBarrioResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class UpdateBarrioCommandHandler(AppDbContext db) : IRequestHandler<UpdateBarrioCommand, Result<UpdateBarrioResponse>>
{
  public async Task<Result<UpdateBarrioResponse>> Handle(UpdateBarrioCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.Barrios.Any(b => EF.Functions.ILike(b.Nombre, dataUpper) && b.Id != request.Id);
    if (exists)
    {
      return Result<UpdateBarrioResponse>.Fail(Error.Conflict("El nombre del barrio ya existe.", "Barrio.Update.Exists"));
    }

    var barrio = db.Barrios.FirstOrDefault(b => b.Id == request.Id);
    if (barrio is null)
    {
      return Result<UpdateBarrioResponse>.Fail(Error.NotFound("Barrio no encontrado.", "Barrio.Update.NotFound"));
    }

    barrio.Nombre = dataUpper;
    await db.SaveChangesAsync(cancellationToken);

    return Result<UpdateBarrioResponse>.Ok(new UpdateBarrioResponse("Barrio actualizado exitosamente."));
  }
}
