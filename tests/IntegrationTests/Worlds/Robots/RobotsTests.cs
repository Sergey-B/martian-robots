// using System.Net;
// using System.Net.Http.Json;
// using Application.Worlds.Get;
// using Shouldly;

// namespace IntegrationTests.Robots;

// public sealed class RobotsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
// {
//     private const string WorldsEndpoint = "worlds";

//     private sealed record RobotResponseDto(int X, int Y, string Orientation, bool Lost);

//     [Fact]
//     public async Task ProcessRobot_Should_ReturnCorrectFinalPosition_WhenMovementIsWithinBounds()
//     {
//         // Arrange - Create a 5x3 world (corresponds to Sample 1 from the requirements)
//         HttpResponseMessage createWorldResponse = await HttpClient.PostAsJsonAsync("api/worlds", new { width = 5, height = 3 });
//         createWorldResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

//         HttpResponseMessage getWorldsResponse = await HttpClient.GetAsync(WorldsEndpoint);
//         getWorldsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

//         List<WorldResponse> worlds = await getWorldsResponse.Content.ReadFromJsonAsync<List<WorldResponse>>();
//         WorldResponse? world = worlds?.FirstOrDefault();

//         var robotRequest = new
//         {
//             x = 1,
//             y = 1,
//             orientation = "E",
//             instructions = "RFRFRFRF"
//         };

//         // Act - Send execution commands for the first robot
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"worlds/{world!.Id}/robots", robotRequest);

//         // Assert - Verify that the robot reached the correct target and remains safe
//         response.EnsureSuccessStatusCode();
//         RobotResponseDto? result = await response.Content.ReadFromJsonAsync<RobotResponseDto>();

//         result.ShouldNotBeNull();
//         result.X.ShouldBe(1);
//         result.Y.ShouldBe(1);
//         result.Orientation.ShouldBe("E");
//         result.Lost.ShouldBeFalse();
//     }

//     [Fact]
//     public async Task ProcessRobot_Should_MarkAsLost_WhenRobotMovesOffTheEdge()
//     {
//         // Arrange - Create a new world and prepare a robot that will fall off (Sample 2 from the requirements)
//         HttpResponseMessage createWorldResponse = await HttpClient.PostAsJsonAsync("api/worlds", new { width = 5, height = 3 });
//         createWorldResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

//         HttpResponseMessage getWorldsResponse = await HttpClient.GetAsync(WorldsEndpoint);
//         getWorldsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

//         List<WorldResponse> worlds = await getWorldsResponse.Content.ReadFromJsonAsync<List<WorldResponse>>();
//         WorldResponse? world = worlds?.FirstOrDefault();

//         var robotRequest = new
//         {
//             x = 3,
//             y = 2,
//             orientation = "N",
//             instructions = "FRRFLLFFRRFLL"
//         };

//         // Act - Send execution commands for the falling robot
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"worlds/{world!.Id}/robots", robotRequest);

//         // Assert - Verify that the robot is reported missing at its last known safe position
//         response.EnsureSuccessStatusCode();
//         RobotResponseDto? result = await response.Content.ReadFromJsonAsync<RobotResponseDto>();

//         result.ShouldNotBeNull();
//         result.X.ShouldBe(3);
//         result.Y.ShouldBe(3);
//         result.Orientation.ShouldBe("N");
//         result.Lost.ShouldBeTrue(); // The robot successfully fell off the grid edge
//     }

//     [Fact]
//     public async Task ProcessRobot_Should_IgnoreScentedInstruction_WhenPreviousRobotWasLostAtSamePoint()
//     {
//         // Arrange - Create a new 5x3 world to isolate test state
//         HttpResponseMessage createWorldResponse = await HttpClient.PostAsJsonAsync("api/worlds", new { width = 5, height = 3 });
//         createWorldResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

//         HttpResponseMessage getWorldsResponse = await HttpClient.GetAsync(WorldsEndpoint);
//         getWorldsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

//         List<WorldResponse> worlds = await getWorldsResponse.Content.ReadFromJsonAsync<List<WorldResponse>>();
//         WorldResponse? world = worlds?.FirstOrDefault();

//         // 1. First robot moves North, drops off the grid at (3,3) and leaves a scent trail
//         var lostRobotRequest = new
//         {
//             x = 3,
//             y = 2,
//             orientation = "N",
//             instructions = "FRRFLLFFRRFLL"
//         };
//         await HttpClient.PostAsJsonAsync($"worlds/{world!.Id}/robots", lostRobotRequest);

//         // 2. Second robot starts its path (Sample 3 from the requirements).
//         // Its instructions attempt to move Forward off the edge from (3,3), 
//         // but it must ignore that specific forward command due to the scent.
//         var safeRobotRequest = new
//         {
//             x = 0,
//             y = 3,
//             orientation = "W",
//             instructions = "LLFFFLFLFL"
//         };

//         // Act - Execute commands for the second robot
//         HttpResponseMessage response = await HttpClient.PostAsJsonAsync($"worlds/{world!.Id}/robots", safeRobotRequest);

//         // Assert - Verify that the second robot survived by ignoring the dangerous command
//         response.EnsureSuccessStatusCode();
//         RobotResponseDto? result = await response.Content.ReadFromJsonAsync<RobotResponseDto>();

//         result.ShouldNotBeNull();
//         result.X.ShouldBe(2);
//         result.Y.ShouldBe(3);
//         result.Orientation.ShouldBe("S");
//         result.Lost.ShouldBeFalse(); // This robot must survive
//     }
// }
