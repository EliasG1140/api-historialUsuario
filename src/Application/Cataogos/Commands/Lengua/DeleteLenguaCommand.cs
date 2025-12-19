using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Lengua;

//* --------------------------------- Command -------------------------------- */
public sealed record DeleteLenguaCommand(int Id) : IRequest<Result<DeleteLenguaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record DeleteLenguaResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class DeleteLenguaCommandHandler(AppDbContext db) : IRequestHandler<DeleteLenguaCommand, Result<DeleteLenguaResponse>>
{
  public async Task<Result<DeleteLenguaResponse>> Handle(DeleteLenguaCommand request, CancellationToken cancellationToken)
  {
    var lengua = await db.Lenguas.FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);
    if (lengua is null)
    {
      return Result<DeleteLenguaResponse>.Fail(Error.NotFound("Lengua no encontrada.", "Lengua.Delete.NotFound"));
    }

    int personasCount = await db.PersonasLengua.CountAsync(pl => pl.LenguaId == request.Id, cancellationToken);
    if (personasCount > 0)
    {
      return Result<DeleteLenguaResponse>.Fail(Error.Conflict($"No se puede borrar la lengua porque hay {personasCount} personas registradas.", "Lengua.Delete.PersonasExist"));
    }

    db.Lenguas.Remove(lengua);
    await db.SaveChangesAsync(cancellationToken);

    return Result<DeleteLenguaResponse>.Ok(new DeleteLenguaResponse("Lengua eliminada exitosamente."));
  }
}
