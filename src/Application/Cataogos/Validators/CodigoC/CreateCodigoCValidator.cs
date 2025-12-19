using Application.Cataogos.Commands.CodigoC;
using FluentValidation;

namespace Application.Cataogos.Validators.CodigoC;

public sealed class CreateCodigoCValidator : AbstractValidator<CreateCodigoCCommand>
{
  public CreateCodigoCValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre del código C es obligatorio.")
      .MaximumLength(100).WithMessage("El nombre del código C no puede exceder los 100 caracteres.");
  }
}
