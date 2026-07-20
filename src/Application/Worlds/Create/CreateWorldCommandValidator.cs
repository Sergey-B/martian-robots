using FluentValidation;

namespace Application.Worlds.Create;

/// <summary>
/// Validator for the <see cref="CreateWorldCommand"/> to ensure incoming world creation payloads 
/// strictly conform to the grid dimension limits specified in the Mars Rover challenge.
/// </summary>
public sealed class CreateWorldCommandValidator : AbstractValidator<CreateWorldCommand>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreateWorldCommandValidator"/> class and registers evaluation rules.
    /// </summary>
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
