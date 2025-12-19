using Application.Cataogos.Commands.CodigoB;
using FluentValidation;

namespace Application.Cataogos.Validators.CodigoB;

public sealed class CreateCodigoBValidator : AbstractValidator<CreateCodigoBCommand>
{
  public CreateCodigoBValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre del código B es obligatorio.")
      .MaximumLength(100).WithMessage("El nombre del código B no puede exceder los 100 caracteres.");
  }
}
