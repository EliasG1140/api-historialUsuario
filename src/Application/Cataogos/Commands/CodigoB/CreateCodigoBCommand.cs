using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoB;

//* --------------------------------- Command -------------------------------- */
public sealed record CreateCodigoBCommand(string Nombre) : IRequest<Result<CreateCodigoBResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record CreateCodigoBResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class CreateCodigoBCommandHandler(AppDbContext db) : IRequestHandler<CreateCodigoBCommand, Result<CreateCodigoBResponse>>
{
  public async Task<Result<CreateCodigoBResponse>> Handle(CreateCodigoBCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.CodigosB.Any(b => EF.Functions.ILike(b.Nombre, dataUpper));
    if (exists)
    {
      return Result<CreateCodigoBResponse>.Fail(Error.Conflict("El código B ya existe.", "CodigoB.Create.Exists"));
    }

    var codigoB = new Domain.Catalogos.CodigoB { Nombre = dataUpper };
    db.CodigosB.Add(codigoB);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreateCodigoBResponse>.Ok(new CreateCodigoBResponse("Código B creado exitosamente."));
  }
}
