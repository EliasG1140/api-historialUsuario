using Application.Cataogos.Commands.Categoria;
using FluentValidation;

namespace Application.Cataogos.Validators.Categoria;

public sealed class UpdateCategoriaValidator : AbstractValidator<UpdateCategoriaCommand>
{
  public UpdateCategoriaValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre de la categoría es obligatorio.")
      .MaximumLength(100).WithMessage("El nombre de la categoría no puede exceder los 100 caracteres.");

    RuleFor(x => x.Minimo)
      .LessThan(x => x.Maximo).WithMessage("El mínimo debe ser menor que el máximo.");

    RuleFor(x => x.Maximo)
      .GreaterThan(x => x.Minimo).WithMessage("El máximo debe ser mayor que el mínimo.");
  }
}
