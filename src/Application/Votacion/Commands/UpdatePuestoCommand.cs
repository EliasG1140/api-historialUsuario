using Infrastructure.Data;
using MediatR;
using Domain.Catalogos;
using Microsoft.EntityFrameworkCore;

namespace Application.Votacion.Commands;

//* ------------------------------- Command ------------------------------- */
public sealed record UpdatePuestoCommand(int PuestoVotacionId, string Nombre, int Mesas) : IRequest<Result<UpdatePuestoResponse>>;

//* ------------------------------ Response ------------------------------- */
public sealed record UpdatePuestoResponse(string Message);

//* ------------------------------- Handler ------------------------------- */
public sealed class UpdatePuestoCommandHandler(AppDbContext db) : IRequestHandler<UpdatePuestoCommand, Result<UpdatePuestoResponse>>
{
  public async Task<Result<UpdatePuestoResponse>> Handle(UpdatePuestoCommand request, CancellationToken cancellationToken)
  {
    var nombrePuesto = request.Nombre.ToUpperInvariant();
    var puesto = await db.PuestosVotacion
        .Include(p => p.MesasVotacion)
        .FirstOrDefaultAsync(p => p.Id == request.PuestoVotacionId, cancellationToken);
    if (puesto == null)
    {
      return Result<UpdatePuestoResponse>.Fail(Error.NotFound("Puesto de votación no encontrado.", "PuestoVotacion.Update.NotFound"));
    }

    var existsNombre = db.PuestosVotacion.Any(p => p.Nombre == nombrePuesto && p.Id != request.PuestoVotacionId);
    if (existsNombre)
    {
      return Result<UpdatePuestoResponse>.Fail(Error.Conflict("El nombre del puesto de votación ya existe.", "PuestoVotacion.Update.Exists"));
    }

    if (request.Mesas < 1)
    {
      return Result<UpdatePuestoResponse>.Fail(Error.Validation("La cantidad de mesas debe ser mayor a 0.", "PuestoVotacion.Update.MesasMin"));
    }

    if (request.Mesas < puesto.MesasVotacion.Count)
    {
      var mesasAEliminar = puesto.MesasVotacion
          .OrderBy(m => m.Id)
          .Skip(request.Mesas)
          .ToList();

      var mesasConPersonas = await db.MesasVotacion
          .Where(m => mesasAEliminar.Select(x => x.Id).Contains(m.Id))
          .AnyAsync(m => db.Personas.Any(p => p.MesaVotacionId == m.Id), cancellationToken);

      if (mesasConPersonas)
      {
        return Result<UpdatePuestoResponse>.Fail(Error.Conflict("No se pueden eliminar mesas que ya tienen personas asociadas.", "PuestoVotacion.Update.MesasConPersonas"));
      }

      db.MesasVotacion.RemoveRange(mesasAEliminar);
    }
    else if (request.Mesas > puesto.MesasVotacion.Count)
    {
      int mesasActuales = puesto.MesasVotacion.Count;
      for (int i = mesasActuales + 1; i <= request.Mesas; i++)
      {
        puesto.MesasVotacion.Add(new MesaVotacion { Nombre = $"MESA {i}" });
      }
    }

    puesto.Nombre = nombrePuesto;

    await db.SaveChangesAsync(cancellationToken);
    return Result<UpdatePuestoResponse>.Ok(new UpdatePuestoResponse("Puesto de votación actualizado exitosamente."));
  }
}
