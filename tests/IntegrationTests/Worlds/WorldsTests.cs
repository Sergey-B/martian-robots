using System.Net;
using System.Net.Http.Json;
using Application.Worlds;
using Application.Worlds.Get;
using Microsoft.AspNetCore.Mvc;

namespace IntegrationTests.Worlds;

public sealed class WorldsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    private const string WorldsEndpoint = "worlds";

    [Fact]
    public async Task CreateWorld_ShouldReturnCreated_WhenValidRequest()
    {
        // Arrange
        var command = new { Width = 5, Height = 3 };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        WorldResponse? world = await response.Content.ReadFromJsonAsync<WorldResponse>();
        world.ShouldNotBeNull();
        world.Id.ShouldNotBe(Guid.Empty);
        world.Width.ShouldBe(command.Width);
        world.Height.ShouldBe(command.Height);
    }

    [Fact]
    public async Task CreateWorld_ShouldReturnBadRequest_WhenWidthIsNegative()
    {
        // Arrange
        var command = new { Width = -1, Height = 3 };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        ValidationProblemDto? problem = await response.Content.ReadFromJsonAsync<ValidationProblemDto>();

        problem.ShouldNotBeNull();
        problem.Status.ShouldBe(400);
        problem.Title.ShouldBe("Validation.General");

        problem.Errors.ShouldNotBeEmpty();
        problem.Errors.Any(e => e.Description.Contains("Width must be between 0 and 50"))
                  .ShouldBeTrue();
    }

    [Fact]
    public async Task CreateWorld_ShouldReturnBadRequest_WhenHeightIsGreaterThanFifty()
    {
        // Arrange
        var command = new { Width = 5, Height = 51 };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        ValidationProblemDto? problem = await response.Content.ReadFromJsonAsync<ValidationProblemDto>();
    
        problem.ShouldNotBeNull();
        problem.Status.ShouldBe(400);
        problem.Title.ShouldBe("Validation.General");
    
        problem.Errors.ShouldNotBeEmpty();
        problem.Errors.Any(e => e.Description.Contains("Height must be between 0 and 50"))
                  .ShouldBeTrue();
    }

    [Fact]
    public async Task CreateWorld_ShouldReturnBadRequest_WhenBothWidthAndHeightAreInvalid()
    {
        // Arrange
        var command = new { Width = 100, Height = -10 };

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        ValidationProblemDto? problem = await response.Content.ReadFromJsonAsync<ValidationProblemDto>();
        problem.ShouldNotBeNull();
        problem.Errors.ShouldNotBeEmpty();

        problem.Errors.Any(e => e.Description.Contains("Height must be between 0 and 50"))
                  .ShouldBeTrue();

        problem.Errors.Any(e => e.Description.Contains("Width must be between 0 and 50"))
                  .ShouldBeTrue();
    }

    #region Response Models

    private sealed record ValidationErrorDto(string Code, string Description, int Type);
    private sealed record ValidationProblemDto(string Type, string Title, int Status, string Detail, List<ValidationErrorDto> Errors);

    #endregion
}
