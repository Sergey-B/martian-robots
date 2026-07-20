using Domain.Robots;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Robots.Send;

public class SendRobotCommandValidator : AbstractValidator<SendRobotCommand>
{
    private static readonly string AllowedCharacters = string.Concat(
        Enum.GetValues<RobotInstruction>().Select(e => (char)e));
        
    private static readonly Regex InstructionsRegex = new(
        $"^[{Regex.Escape(AllowedCharacters)}]*$", 
        RegexOptions.Compiled);

    public SendRobotCommandValidator()
    {
        RuleFor(cmd => cmd.WorldId)
            .NotEmpty().WithMessage("WorldId is required.");

        RuleFor(cmd => cmd.X)
            .InclusiveBetween(0, 50)
            .WithMessage("X must be between 0 and 50.");

        RuleFor(cmd => cmd.Y)
            .InclusiveBetween(0, 50)
            .WithMessage("Y must be between 0 and 50.");

        RuleFor(cmd => cmd.Orientation)
            .IsEnumName(typeof(RobotOrientation), caseSensitive: false)
            .WithMessage($"Orientation must be one of: {string.Join(", ", Enum.GetNames<RobotOrientation>())}.");

        RuleFor(cmd => cmd.Instructions)
            .MaximumLength(100).WithMessage("Instructions cannot exceed 100 characters.")
            .Matches(InstructionsRegex)
            .WithMessage($"Instructions can only contain characters from the allowed set: {string.Join(", ", AllowedCharacters.ToCharArray())}.");
    }
}
