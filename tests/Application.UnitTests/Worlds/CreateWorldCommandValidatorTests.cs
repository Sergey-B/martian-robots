using Application.Worlds.Create;
using FluentValidation.TestHelper;
using Xunit;

namespace Application.UnitTests.Worlds.Create;

public class CreateWorldCommandValidatorTests
{
    private readonly CreateWorldCommandValidator _validator;

    public CreateWorldCommandValidatorTests()
    {
        _validator = new CreateWorldCommandValidator();
    }

    /// <summary>
    /// Helper method to build a baseline valid command.
    /// </summary>
    private static CreateWorldCommand CreateValidCommand() =>
        new(Width: 5, Height: 3);

    [Theory]
    [InlineData(0, 0)]   // Minimum allowed boundary
    [InlineData(50, 50)] // Maximum allowed boundary
    [InlineData(25, 25)] // Middle valid value
    public void Should_NotHaveAnyErrors_When_DimensionsAreWithinBounds(int width, int height)
    {
        // Arrange
        var command = new CreateWorldCommand(width, height);

        // Act
        TestValidationResult<CreateWorldCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(51)]
    [InlineData(100)]
    public void Should_HaveError_When_WidthIsOutOfBounds(int invalidWidth)
    {
        // Arrange
        CreateWorldCommand command = CreateValidCommand() with { Width = invalidWidth };

        // Act
        TestValidationResult<CreateWorldCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Width)
            .WithErrorMessage("Width must be between 0 and 50.");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(51)]
    [InlineData(200)]
    public void Should_HaveError_When_HeightIsOutOfBounds(int invalidHeight)
    {
        // Arrange
        CreateWorldCommand command = CreateValidCommand() with { Height = invalidHeight };

        // Act
        TestValidationResult<CreateWorldCommand> result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Height)
            .WithErrorMessage("Height must be between 0 and 50.");
    }
}
