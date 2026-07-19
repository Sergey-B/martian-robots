using Application.Robots.Send;
using Application.UnitTests.Abstractions;
using Domain.Robots;
using Domain.Scents;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Robots;

public sealed class SendRobotCommandHandlerTests : BaseHandlerTest
{
    private static readonly Guid WorldId = Guid.NewGuid();

    private static SendRobotCommand CreateCommand(string orientation, string instructions) => 
        new(
            WorldId: WorldId,
            X: 1,
            Y: 1,
            Orientation: orientation,
            Instructions: instructions
        );

    [Fact]
    public async Task Handle_Should_ReturnNotFound_WhenWorldDoesNotExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        SendRobotCommand command = CreateCommand("N", "LFF");
        var handler = new SendRobotCommandHandler(context);

        // Act
        Result<RobotResponse> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(WorldErrors.NotFound(WorldId));
    }

    [Fact]
    public async Task Handle_Should_ReturnInvalidOrientation_WhenOrientationIsInvalid()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();

        World world = World.Create(5, 5).Value;
        context.Worlds.Add(world);
        await context.SaveChangesAsync();

        SendRobotCommand command = CreateCommand("INVALID", "LFF");
        var handler = new SendRobotCommandHandler(context);

        // Act
        Result<RobotResponse> result = await handler.Handle(command with { WorldId = world.Id }, CancellationToken.None);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.ShouldBe(RobotErrors.InvalidOrientation);
    }

    [Fact]
    public async Task Handle_Should_MoveRobotAndNotSaveChanges_WhenRobotStaysInBounds()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();

        World world = World.Create(5, 5).Value;
        context.Worlds.Add(world);
        await context.SaveChangesAsync();

        var command = new SendRobotCommand(world.Id, X: 1, Y: 1, Orientation: "E", Instructions: "F");
        var handler = new SendRobotCommandHandler(context);

        // Act
        Result<RobotResponse> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.X.ShouldBe(2);
        result.Value.Y.ShouldBe(1);
        result.Value.Orientation.ShouldBe("E");
        result.Value.Lost.ShouldBeFalse();

        int scentCount = await context.Scents.CountAsync();
        scentCount.ShouldBe(0);
    }

    [Fact]
    public async Task Handle_Should_PersistNewScentAndMarkAsLost_WhenRobotMovesOutOfBounds()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();

        // Create small world 1х1
        World world = World.Create(1, 1).Value;
        context.Worlds.Add(world);
        await context.SaveChangesAsync();

        // Robots starts on the edget (1, 1) to North (N) and make step forward -> lost
        var command = new SendRobotCommand(world.Id, X: 1, Y: 1, Orientation: "N", Instructions: "F");
        var handler = new SendRobotCommandHandler(context);

        // Act
        Result<RobotResponse> result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.X.ShouldBe(1); // last valid position
        result.Value.Y.ShouldBe(1);
        result.Value.Orientation.ShouldBe("N");
        result.Value.Lost.ShouldBeTrue();

        Scent persistedScent = await context.Scents.SingleAsync();
        persistedScent.WorldId.ShouldBe(world.Id);
        persistedScent.X.ShouldBe(1);
        persistedScent.Y.ShouldBe(1);
    }
}
