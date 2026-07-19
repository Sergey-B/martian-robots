using Application.Worlds.Get;
using Application.UnitTests.Abstractions;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;
using Shouldly;
using Xunit;

namespace Application.UnitTests.Worlds.Get;

public sealed class GetWorldsQueryHandlerTests : BaseHandlerTest
{
    [Fact]
    public async Task Handle_Should_ReturnEmptyList_WhenNoWorldsExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();
        var query = new GetWorldsQuery();
        var handler = new GetWorldsQueryHandler(context);

        // Act
        Result<List<WorldResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_Should_ReturnMappedWorldResponses_WhenWorldsExist()
    {
        // Arrange
        await using TestDbContext context = CreateDbContext();

        // Create 2 worlds with different size
        World world1 = World.Create(3, 5).Value;
        World world2 = World.Create(25, 50).Value;

        context.Worlds.AddRange(world1, world2);
        await context.SaveChangesAsync();

        var query = new GetWorldsQuery();
        var handler = new GetWorldsQueryHandler(context);

        // Act
        Result<List<WorldResponse>> result = await handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.ShouldBeTrue();
        result.Value.Count.ShouldBe(2);

        WorldResponse firstResponse = result.Value.Single(w => w.Id == world1.Id);
        firstResponse.Width.ShouldBe(5);
        firstResponse.Height.ShouldBe(3);

        WorldResponse secondResponse = result.Value.Single(w => w.Id == world2.Id);
        secondResponse.Width.ShouldBe(50);
        secondResponse.Height.ShouldBe(25);
    }
}
