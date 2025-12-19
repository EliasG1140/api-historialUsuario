using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoC;

//* --------------------------------- Command -------------------------------- */
public sealed record UpdateCodigoCCommand(int Id, string Nombre) : IRequest<Result<UpdateCodigoCResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record UpdateCodigoCResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class UpdateCodigoCCommandHandler(AppDbContext db) : IRequestHandler<UpdateCodigoCCommand, Result<UpdateCodigoCResponse>>
{
  public async Task<Result<UpdateCodigoCResponse>> Handle(UpdateCodigoCCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.CodigosC.Any(c => EF.Functions.ILike(c.Nombre, dataUpper) && c.Id != request.Id);
    if (exists)
    {
      return Result<UpdateCodigoCResponse>.Fail(Error.Conflict("El nombre del código C ya existe.", "CodigoC.Update.Exists"));
    }

    var codigoC = db.CodigosC.FirstOrDefault(c => c.Id == request.Id);
    if (codigoC is null)
    {
      return Result<UpdateCodigoCResponse>.Fail(Error.NotFound("Código C no encontrado.", "CodigoC.Update.NotFound"));
    }

    codigoC.Nombre = dataUpper;
    await db.SaveChangesAsync(cancellationToken);

    return Result<UpdateCodigoCResponse>.Ok(new UpdateCodigoCResponse("Código C actualizado exitosamente."));
  }
}
