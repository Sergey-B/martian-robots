using FluentValidation;

namespace Application.Worlds.Create;

public class CreateWorldCommandValidator : AbstractValidator<CreateWorldCommand>
{
    public CreateWorldCommandValidator()
    {
        RuleFor(cmd => cmd.Width)
            .InclusiveBetween(0, 50)
            .WithMessage("Width must be between 0 and 50.");

        RuleFor(cmd => cmd.Height)
            .InclusiveBetween(0, 50)
            .WithMessage("Height must be between 0 and 50.");
    }
}
