using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoB;

//* --------------------------------- Command -------------------------------- */
public sealed record UpdateCodigoBCommand(int Id, string Nombre) : IRequest<Result<UpdateCodigoBResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record UpdateCodigoBResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class UpdateCodigoBCommandHandler(AppDbContext db) : IRequestHandler<UpdateCodigoBCommand, Result<UpdateCodigoBResponse>>
{
  public async Task<Result<UpdateCodigoBResponse>> Handle(UpdateCodigoBCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.CodigosB.Any(b => EF.Functions.ILike(b.Nombre, dataUpper) && b.Id != request.Id);
    if (exists)
    {
      return Result<UpdateCodigoBResponse>.Fail(Error.Conflict("El nombre del código B ya existe.", "CodigoB.Update.Exists"));
    }

    var codigoB = db.CodigosB.FirstOrDefault(b => b.Id == request.Id);
    if (codigoB is null)
    {
      return Result<UpdateCodigoBResponse>.Fail(Error.NotFound("Código B no encontrado.", "CodigoB.Update.NotFound"));
    }

    codigoB.Nombre = dataUpper;
    await db.SaveChangesAsync(cancellationToken);

    return Result<UpdateCodigoBResponse>.Ok(new UpdateCodigoBResponse("Código B actualizado exitosamente."));
  }
}
