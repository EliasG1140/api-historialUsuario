using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Lengua;

//* --------------------------------- Command -------------------------------- */
public sealed record UpdateLenguaCommand(int Id, string Nombre) : IRequest<Result<UpdateLenguaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record UpdateLenguaResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class UpdateLenguaCommandHandler(AppDbContext db) : IRequestHandler<UpdateLenguaCommand, Result<UpdateLenguaResponse>>
{
  public async Task<Result<UpdateLenguaResponse>> Handle(UpdateLenguaCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.Lenguas.Any(l => EF.Functions.ILike(l.Nombre, dataUpper) && l.Id != request.Id);
    if (exists)
    {
      return Result<UpdateLenguaResponse>.Fail(Error.Conflict("El nombre de la lengua ya existe.", "Lengua.Update.Exists"));
    }

    var lengua = db.Lenguas.FirstOrDefault(l => l.Id == request.Id);
    if (lengua is null)
    {
      return Result<UpdateLenguaResponse>.Fail(Error.NotFound("Lengua no encontrada.", "Lengua.Update.NotFound"));
    }

    lengua.Nombre = dataUpper;
    await db.SaveChangesAsync(cancellationToken);

    return Result<UpdateLenguaResponse>.Ok(new UpdateLenguaResponse("Lengua actualizada exitosamente."));
  }
}
