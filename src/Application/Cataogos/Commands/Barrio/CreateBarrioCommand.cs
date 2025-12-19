using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Barrio;

//* --------------------------------- Command -------------------------------- */
public sealed record CreateBarrioCommand(string Nombre) : IRequest<Result<CreateBarrioResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record CreateBarrioResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class CreateBarrioCommandHandler(AppDbContext db) : IRequestHandler<CreateBarrioCommand, Result<CreateBarrioResponse>>
{
  public async Task<Result<CreateBarrioResponse>> Handle(CreateBarrioCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.Barrios.Any(b => EF.Functions.ILike(b.Nombre, dataUpper));
    if (exists)
    {
      return Result<CreateBarrioResponse>.Fail(Error.Conflict("El barrio ya existe.", "Barrio.Create.Exists"));
    }

    var barrio = new Domain.Catalogos.Barrio { Nombre = dataUpper };
    db.Barrios.Add(barrio);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreateBarrioResponse>.Ok(new CreateBarrioResponse("Barrio creado exitosamente."));
  }
}