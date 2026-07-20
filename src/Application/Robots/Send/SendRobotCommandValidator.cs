using Domain.Robots;
using FluentValidation;
using System.Text.RegularExpressions;

namespace Application.Robots.Send;

/// <summary>
/// Validator for the <see cref="SendRobotCommand"/> to ensure incoming HTTP payload metrics 
/// adhere strictly to the business rule constraints defined by the Mars Rover challenge specifications.
/// </summary>
public sealed class SendRobotCommandValidator : AbstractValidator<SendRobotCommand>
{
    /// <summary>
    /// Dynamically generated string listing all valid character values extracted from the <see cref="RobotInstruction"/> enum.
    /// </summary>
    private static readonly string AllowedCharacters = string.Concat(
        Enum.GetValues<RobotInstruction>().Select(e => (char)e));
        
    /// <summary>
    /// Compiled regular expression used to validate that the instructions string strictly contains allowed execution tokens.
    /// </summary>
    private static readonly Regex InstructionsRegex = new(
        $"^[{Regex.Escape(AllowedCharacters)}]*$", 
        RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="SendRobotCommandValidator"/> class and registers evaluation expressions.
    /// </summary>
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
