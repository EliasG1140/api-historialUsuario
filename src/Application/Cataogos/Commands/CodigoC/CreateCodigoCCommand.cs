using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.CodigoC;

//* --------------------------------- Command -------------------------------- */
public sealed record CreateCodigoCCommand(string Nombre) : IRequest<Result<CreateCodigoCResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record CreateCodigoCResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class CreateCodigoCCommandHandler(AppDbContext db) : IRequestHandler<CreateCodigoCCommand, Result<CreateCodigoCResponse>>
{
  public async Task<Result<CreateCodigoCResponse>> Handle(CreateCodigoCCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.CodigosC.Any(c => EF.Functions.ILike(c.Nombre, dataUpper));
    if (exists)
    {
      return Result<CreateCodigoCResponse>.Fail(Error.Conflict("El código C ya existe.", "CodigoC.Create.Exists"));
    }

    var codigoC = new Domain.Catalogos.CodigoC { Nombre = dataUpper };
    db.CodigosC.Add(codigoC);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreateCodigoCResponse>.Ok(new CreateCodigoCResponse("Código C creado exitosamente."));
  }
}
