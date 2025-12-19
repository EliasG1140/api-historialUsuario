using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Cataogos.Commands.Lengua;

//* --------------------------------- Command -------------------------------- */
public sealed record CreateLenguaCommand(string Nombre) : IRequest<Result<CreateLenguaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record CreateLenguaResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class CreateLenguaCommandHandler(AppDbContext db) : IRequestHandler<CreateLenguaCommand, Result<CreateLenguaResponse>>
{
  public async Task<Result<CreateLenguaResponse>> Handle(CreateLenguaCommand request, CancellationToken cancellationToken)
  {
    var dataUpper = request.Nombre.ToUpperInvariant();
    var exists = db.Lenguas.Any(l => EF.Functions.ILike(l.Nombre, dataUpper));
    if (exists)
    {
      return Result<CreateLenguaResponse>.Fail(Error.Conflict("La lengua ya existe.", "Lengua.Create.Exists"));
    }

    var lengua = new Domain.Catalogos.Lengua { Nombre = dataUpper };
    db.Lenguas.Add(lengua);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreateLenguaResponse>.Ok(new CreateLenguaResponse("Lengua creada exitosamente."));
  }
}
