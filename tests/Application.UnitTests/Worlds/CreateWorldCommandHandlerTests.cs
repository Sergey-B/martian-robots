using Application.Worlds.Create;
using Application.UnitTests.Abstractions;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Shouldly;
using Xunit;
using Application.Worlds;

namespace Application.UnitTests.Worlds;

public sealed class CreateWorldCommandHandlerTests : BaseHandlerTest
{
    private static CreateWorldCommand Command => new(Width: 5, Height: 3);

    [Fact]
    public async Task Handle_Should_ReturnFailure_WhenDomainValidationFails()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        
        var invalidCommand = new CreateWorldCommand(Width: 100, Height: 3);
        var handler = new CreateWorldCommandHandler(context);

        // Act
        Result<WorldResponse> result = await handler.Handle(invalidCommand, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(WorldErrors.InvalidWidth); // Ожидаем доменную ошибку валидации
    }

    [Fact]
    public async Task Handle_Should_PersistWorld_WhenDimensionsAreValidAndWorldIsNew()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var handler = new CreateWorldCommandHandler(context);

        // Act
        Result<WorldResponse> result = await handler.Handle(Command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();

        World persistedWorld = await context.Worlds.SingleAsync(w => w.Id == result.Value.Id);
        persistedWorld.Width.ShouldBe(5);
        persistedWorld.Height.ShouldBe(3);
    }
}
