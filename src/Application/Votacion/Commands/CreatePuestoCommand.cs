using Infrastructure.Data;
using MediatR;
using Domain.Catalogos;

namespace Application.Votacion.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record CreatePuestoCommand(string Nombre, int Mesas) : IRequest<Result<CreatePuestoResponse>>;

//* ------------------------------ Response ------------------------------- */
public sealed record CreatePuestoResponse(int PuestoVotacionId, string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class CreatePuestoCommandHandler(AppDbContext db) : IRequestHandler<CreatePuestoCommand, Result<CreatePuestoResponse>>
{
  public async Task<Result<CreatePuestoResponse>> Handle(CreatePuestoCommand request, CancellationToken cancellationToken)
  {
    var existsNombre = db.PuestosVotacion.Any(p => p.Nombre == request.Nombre);
    if (existsNombre)
    {
      return Result<CreatePuestoResponse>.Fail(Error.Conflict("El nombre del puesto de votación ya existe.", "PuestoVotacion.Create.Exists"));
    }

    if (request.Mesas < 1)
    {
      return Result<CreatePuestoResponse>.Fail(Error.Validation("La cantidad de mesas debe ser mayor a 0.", "PuestoVotacion.Create.MesasMin"));
    }

    var puesto = new PuestoVotacion { Nombre = request.Nombre };

    for (int i = 1; i <= request.Mesas; i++)
    {
      puesto.MesasVotacion.Add(new MesaVotacion { Nombre = $"Mesa {i}" });
    }

    db.PuestosVotacion.Add(puesto);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreatePuestoResponse>.Ok(new CreatePuestoResponse(puesto.Id, "Puesto de votación creado exitosamente."));
  }
}
