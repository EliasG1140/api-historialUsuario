using Infrastructure.Data;
using MediatR;

namespace Application.Cataogos.Commands.Categoria;

//* --------------------------------- Command -------------------------------- */
public sealed record DeleteCategoriaCommand(int Id) : IRequest<Result<DeleteCategoriaResponse>>;

//* -------------------------------- Response -------------------------------- */
public sealed record DeleteCategoriaResponse(string Message);

//* --------------------------------- Handler --------------------------------- */
public sealed class DeleteCategoriaCommandHandler(AppDbContext db) : IRequestHandler<DeleteCategoriaCommand, Result<DeleteCategoriaResponse>>
{
  public async Task<Result<DeleteCategoriaResponse>> Handle(DeleteCategoriaCommand request, CancellationToken cancellationToken)
  {
    var categoria = await db.Categorias.FindAsync([request.Id], cancellationToken);
    if (categoria is null)
    {
      return Result<DeleteCategoriaResponse>.Fail(Error.NotFound("Categoría no encontrada.", "Categoria.Delete.NotFound"));
    }

    db.Categorias.Remove(categoria);
    await db.SaveChangesAsync(cancellationToken);

    return Result<DeleteCategoriaResponse>.Ok(new DeleteCategoriaResponse("Categoría eliminada exitosamente."));
  }
}
