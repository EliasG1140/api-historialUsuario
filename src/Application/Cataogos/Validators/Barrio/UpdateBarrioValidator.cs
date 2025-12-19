using Application.Cataogos.Commands.Barrio;
using FluentValidation;

namespace Application.Cataogos.Validators.Barrio;

public sealed class UpdateBarrioValidator : AbstractValidator<UpdateBarrioCommand>
{
  public UpdateBarrioValidator()
  {
    RuleFor(x => x.Nombre)
      .NotEmpty().WithMessage("El nombre del barrio es obligatorio.")
      .MaximumLength(100).WithMessage("El nombre del barrio no puede exceder los 100 caracteres.");
  }
}
