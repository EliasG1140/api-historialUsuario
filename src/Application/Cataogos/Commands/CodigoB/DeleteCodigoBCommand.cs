using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoB;

//* --------------------------------- Command -------------------------------- */
public sealed record DeleteCodigoBCommand(int Id) : IRequest<Result<DeleteCodigoBResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record DeleteCodigoBResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class DeleteCodigoBCommandHandler(AppDbContext db) : IRequestHandler<DeleteCodigoBCommand, Result<DeleteCodigoBResponse>>
{
  public async Task<Result<DeleteCodigoBResponse>> Handle(DeleteCodigoBCommand request, CancellationToken cancellationToken)
  {
    var codigoB = await db.CodigosB.FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);
    if (codigoB is null)
    {
      return Result<DeleteCodigoBResponse>.Fail(Error.NotFound("Código B no encontrado.", "CodigoB.Delete.NotFound"));
    }

    var relaciones = db.Set<Domain.Personas.PersonaCodigoB>().Where(pc => pc.CodigoBId == request.Id);
    db.Set<Domain.Personas.PersonaCodigoB>().RemoveRange(relaciones);

    db.CodigosB.Remove(codigoB);
    await db.SaveChangesAsync(cancellationToken);

    return Result<DeleteCodigoBResponse>.Ok(new DeleteCodigoBResponse("Código B y sus relaciones eliminados exitosamente."));
  }
}
