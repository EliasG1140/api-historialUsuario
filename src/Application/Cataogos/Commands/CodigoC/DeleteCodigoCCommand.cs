using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoC;

//* --------------------------------- Command -------------------------------- */
public sealed record DeleteCodigoCCommand(int Id) : IRequest<Result<DeleteCodigoCResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record DeleteCodigoCResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class DeleteCodigoCCommandHandler(AppDbContext db) : IRequestHandler<DeleteCodigoCCommand, Result<DeleteCodigoCResponse>>
{
  public async Task<Result<DeleteCodigoCResponse>> Handle(DeleteCodigoCCommand request, CancellationToken cancellationToken)
  {
    var codigoC = await db.CodigosC.Include(c => c.Personas).FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    if (codigoC is null)
    {
      return Result<DeleteCodigoCResponse>.Fail(Error.NotFound("Código C no encontrado.", "CodigoC.Delete.NotFound"));
    }

    int personasCount = codigoC.Personas?.Count ?? 0;
    if (personasCount > 0)
    {
      return Result<DeleteCodigoCResponse>.Fail(Error.Conflict($"No se puede borrar el código C porque hay {personasCount} personas registradas.", "CodigoC.Delete.PersonasExist"));
    }

    db.CodigosC.Remove(codigoC);
    await db.SaveChangesAsync(cancellationToken);

    return Result<DeleteCodigoCResponse>.Ok(new DeleteCodigoCResponse("Código C eliminado exitosamente."));
  }
}
