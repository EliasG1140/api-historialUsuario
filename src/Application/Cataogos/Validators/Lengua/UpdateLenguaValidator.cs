using Application.Cataogos.Commands.Lengua;
using FluentValidation;

namespace Application.Cataogos.Validators.Lengua;

public sealed class UpdateLenguaValidator : AbstractValidator<UpdateLenguaCommand>
{
  public UpdateLenguaValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre de la lengua es obligatorio.")
      .MaximumLength(100).WithMessage("El nombre de la lengua no puede exceder los 100 caracteres.");
  }
}
