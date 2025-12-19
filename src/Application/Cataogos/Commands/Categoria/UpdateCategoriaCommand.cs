using Infrastructure.Data;
using MediatR;

namespace Application.Cataogos.Commands.Categoria;

//* --------------------------------- Command -------------------------------- */
public sealed record UpdateCategoriaCommand(int Id, string Nombre, int Minimo, int Maximo) : IRequest<Result<UpdateCategoriaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record UpdateCategoriaResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class UpdateCategoriaCommandHandler(AppDbContext db) : IRequestHandler<UpdateCategoriaCommand, Result<UpdateCategoriaResponse>>
{
  public async Task<Result<UpdateCategoriaResponse>> Handle(UpdateCategoriaCommand request, CancellationToken cancellationToken)
  {
    // 1. Validar nombre único (excluyendo el propio Id)
    var existsNombre = db.Categorias.Any(c => c.Nombre == request.Nombre && c.Id != request.Id);
    if (existsNombre)
    {
      return Result<UpdateCategoriaResponse>.Fail(Error.Conflict("El nombre de la categoría ya existe.", "Categoria.Update.Exists"));
    }

    // 2. Validar rango mínimo/máximo
    if (request.Minimo >= request.Maximo)
    {
      return Result<UpdateCategoriaResponse>.Fail(Error.Validation("El mínimo debe ser menor que el máximo.", "Categoria.Update.RangoInvalido"));
    }

    // 3. Validar que el rango no se solape con otra categoría (excluyendo el propio Id)
    var existsRango = db.Categorias.Any(c =>
      c.Id != request.Id && (
        (request.Minimo >= c.Minimo && request.Minimo <= c.Maximo) ||
        (request.Maximo >= c.Minimo && request.Maximo <= c.Maximo) ||
        (request.Minimo <= c.Minimo && request.Maximo >= c.Maximo)
      )
    );
    if (existsRango)
    {
      return Result<UpdateCategoriaResponse>.Fail(Error.Conflict("El rango de la categoría se solapa con otra existente.", "Categoria.Update.RangoSolapado"));
    }

    var categoria = db.Categorias.FirstOrDefault(c => c.Id == request.Id);
    if (categoria is null)
    {
      return Result<UpdateCategoriaResponse>.Fail(Error.NotFound("Categoría no encontrada.", "Categoria.Update.NotFound"));
    }

    categoria.Nombre = request.Nombre;
    categoria.Minimo = request.Minimo;
    categoria.Maximo = request.Maximo;
    await db.SaveChangesAsync(cancellationToken);

    return Result<UpdateCategoriaResponse>.Ok(new UpdateCategoriaResponse("Categoría actualizada exitosamente."));
  }
}
