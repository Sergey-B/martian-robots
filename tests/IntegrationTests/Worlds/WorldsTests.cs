// using System.Net;
// using System.Net.Http.Json;
// using Application.Worlds.Get;

// namespace IntegrationTests.Worlds;

// public sealed class WorldsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
// {
//     private const string WorldsEndpoint = "worlds";

//     [Fact]
//     public async Task CreateWorld_ShouldReturnCreated_WhenValidRequest()
//     {
//         // Arrange
//         var command = new { Width = 5, Height = 3 };

//         // Act
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.Created);
//         response.Headers.Location.ShouldNotBeNull();

//         WorldResponse? world = await response.Content.ReadFromJsonAsync<WorldResponse>();
//         world.ShouldNotBeNull();
//         world.Id.ShouldNotBe(Guid.Empty);
//         world.Width.ShouldBe(command.Width);
//         world.Height.ShouldBe(command.Height);
//     }

//     [Fact]
//     public async Task CreateWorld_ShouldReturnBadRequest_WhenWidthIsNegative()
//     {
//         // Arrange
//         var command = new { Width = -1, Height = 3 };

//         // Act
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//         ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
//         problem.ShouldNotBeNull();
//         problem.Title.ShouldContain("Validation");
//     }

//     [Fact]
//     public async Task CreateWorld_ShouldReturnBadRequest_WhenHeightIsGreaterThanFifty()
//     {
//         // Arrange
//         var command = new { Width = 5, Height = 51 };

//         // Act
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//         ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
//         problem.ShouldNotBeNull();
//         problem.Title.ShouldContain("Validation");
//     }

//     [Fact]
//     public async Task CreateWorld_ShouldReturnBadRequest_WhenBothWidthAndHeightAreInvalid()
//     {
//         // Arrange
//         var command = new { Width = 100, Height = -10 };

//         // Act
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync(WorldsEndpoint, command);

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
//         ProblemDetails? problem = await response.Content.ReadFromJsonAsync<ProblemDetails>();
//         problem.ShouldNotBeNull();
//         problem.Errors.ShouldNotBeNull();
//         problem.Errors.ShouldContainKey("Width");
//         problem.Errors.ShouldContainKey("Height");
//     }

//     #region Get World

//     [Fact]
//     public async Task GetWorld_ShouldReturnWorld_WhenWorldExists()
//     {
//         // Arrange
//         WorldResponse created = await CreateWorldAsync(5, 3);

//         // Act
//         HttpResponseMessage response = await HttpClient.GetAsync($"{WorldsEndpoint}/{created.Id}");

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.OK);
//         WorldResponse? world = await response.Content.ReadFromJsonAsync<WorldResponse>();
//         world.ShouldNotBeNull();
//         world.Id.ShouldBe(created.Id);
//         world.Width.ShouldBe(created.Width);
//         world.Height.ShouldBe(created.Height);
//     }

//     [Fact]
//     public async Task GetWorld_ShouldReturnNotFound_WhenWorldDoesNotExist()
//     {
//         // Arrange
//         var nonExistentId = Guid.NewGuid();

//         // Act
//         HttpResponseMessage response = await HttpClient.GetAsync($"{WorldsEndpoint}/{nonExistentId}");

//         // Assert
//         response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
//     }

//     #endregion

//     #region Helpers

//     private async Task<WorldResponse> CreateWorldAsync(int worldWidth, int worldHeight)
//     {
//         HttpResponseMessage createWorldResponse = await HttpClient.PostAsJsonAsync(WorldsEndpoint, new { width = worldWidth, height = worldHeight });
//         createWorldResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

//         HttpResponseMessage getWorldsResponse = await HttpClient.GetAsync(WorldsEndpoint);
//         getWorldsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

//         List<WorldResponse> worlds = await getWorldsResponse.Content.ReadFromJsonAsync<List<WorldResponse>>();
//         return worlds?.FirstOrDefault();
//     }

//     #endregion

//     #region Response Models

//     private sealed record ProblemDetails(string Title, IDictionary<string, string[]>? Errors);

//     #endregion
// }
