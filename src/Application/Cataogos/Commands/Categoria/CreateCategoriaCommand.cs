using Infrastructure.Data;
using MediatR;

namespace Application.Cataogos.Commands.Categoria;

//* --------------------------------- Command -------------------------------- */
public sealed record CreateCategoriaCommand(string Nombre, int Minimo, int Maximo) : IRequest<Result<CreateCategoriaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record CreateCategoriaResponse(string Message);

//* --------------------------------- Handle --------------------------------- */
public sealed class CreateCategoriaCommandHandler(AppDbContext db) : IRequestHandler<CreateCategoriaCommand, Result<CreateCategoriaResponse>>
{
  public async Task<Result<CreateCategoriaResponse>> Handle(CreateCategoriaCommand request, CancellationToken cancellationToken)
  {
    // 1. Validar nombre único
    var existsNombre = db.Categorias.Any(c => c.Nombre == request.Nombre);
    if (existsNombre)
    {
      return Result<CreateCategoriaResponse>.Fail(Error.Conflict("El nombre de la categoría ya existe.", "Categoria.Create.Exists"));
    }

    // 2. Validar rango mínimo/máximo
    if (request.Minimo >= request.Maximo)
    {
      return Result<CreateCategoriaResponse>.Fail(Error.Validation("El mínimo debe ser menor que el máximo.", "Categoria.Create.RangoInvalido"));
    }

    // 3. Validar que el rango no se solape con otra categoría
    var existsRango = db.Categorias.Any(c =>
      (request.Minimo >= c.Minimo && request.Minimo <= c.Maximo) ||
      (request.Maximo >= c.Minimo && request.Maximo <= c.Maximo) ||
      (request.Minimo <= c.Minimo && request.Maximo >= c.Maximo)
    );
    if (existsRango)
    {
      return Result<CreateCategoriaResponse>.Fail(Error.Conflict("El rango de la categoría se solapa con otra existente.", "Categoria.Create.RangoSolapado"));
    }

    var categoria = new Domain.Catalogos.Categoria { Nombre = request.Nombre, Minimo = request.Minimo, Maximo = request.Maximo };
    db.Categorias.Add(categoria);
    await db.SaveChangesAsync(cancellationToken);

    return Result<CreateCategoriaResponse>.Ok(new CreateCategoriaResponse("Categoría creada exitosamente."));
  }
}
