using Application.Robots.Send;
using Domain.Robots;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.UnitTests.Robots.Send;

public class SendRobotCommandValidatorTests
{
    private readonly SendRobotCommandValidator _validator;

    public SendRobotCommandValidatorTests()
    {
        _validator = new SendRobotCommandValidator();
    }

    private static SendRobotCommand CreateValidCommand() =>
        new(
            WorldId: Guid.NewGuid(),
            X: 5,
            Y: 10,
            Orientation: "N",
            Instructions: "LFFRFL"
        );

    [Fact]
    public void Should_NotHaveAnyErrors_When_CommandIsValid()
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand();

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_HaveError_When_WorldIdIsEmpty()
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { WorldId = Guid.Empty };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.WorldId)
            .WithErrorMessage("WorldId is required.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Should_HaveError_When_X_IsNegative(int invalidX)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { X = invalidX };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.X)
            .WithErrorMessage("X must be between 0 and 50.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-50)]
    public void Should_HaveError_When_Y_IsNegative(int invalidY)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { Y = invalidY };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Y)
            .WithErrorMessage("Y must be between 0 and 50.");
    }

    [Theory]
    [InlineData("N")]
    [InlineData("S")]
    [InlineData("E")]
    [InlineData("W")]
    [InlineData("n")]
    [InlineData("s")]
    public void Should_NotHaveError_When_OrientationIsValid(string validOrientation)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { Orientation = validOrientation };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Orientation);
    }

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    [InlineData("North")]
    [InlineData("1")]
    public void Should_HaveError_When_OrientationIsInvalid(string invalidOrientation)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { Orientation = invalidOrientation };
        string expectedMessage = $"Orientation must be one of: {string.Join(", ", Enum.GetNames<RobotOrientation>())}.";

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Orientation)
            .WithErrorMessage(expectedMessage);
    }

    [Theory]
    [InlineData("L")]
    [InlineData("R")]
    [InlineData("F")]
    [InlineData("LRF")]
    [InlineData("")]
    public void Should_NotHaveError_When_InstructionsAreValid(string validInstructions)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { Instructions = validInstructions };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveValidationErrorFor(x => x.Instructions);
    }

    [Theory]
    [InlineData("LAF")] // Символ 'A' недопустим
    [InlineData("L R")] // Пробелы недопустимы
    [InlineData("lrf")] // Маленькие буквы недопустимы (регулярное выражение требует заглавные)
    public void Should_HaveError_When_InstructionsContainInvalidCharacters(string invalidInstructions)
    {
        // Arrange
        SendRobotCommand command = CreateValidCommand() with { Instructions = invalidInstructions };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Instructions)
            .WithErrorMessage("Instructions can only contain characters from the allowed set: F, L, R.");
    }

    [Fact]
    public void Should_HaveError_When_InstructionsExceedMaximumLength()
    {
        // Arrange
        string longInstructions = new string('F', 101); // 101 символ 'F'
        SendRobotCommand command = CreateValidCommand() with { Instructions = longInstructions };

        // Act
        TestValidationResult<SendRobotCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Instructions)
            .WithErrorMessage("Instructions cannot exceed 100 characters.");
    }
}
