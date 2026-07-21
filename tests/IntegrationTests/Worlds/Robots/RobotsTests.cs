using System.Net;
using System.Net.Http.Json;
using Application.Robots.Send;
using Application.Worlds;
using Application.Worlds.Get;
using Shouldly;

namespace IntegrationTests.Robots;

public sealed class RobotsTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task ProcessRobot_Should_ExecuteMartianRobotsScenariosCorrectly()
    {
        // Arrange - Create world 5х3 (Sample 1, 2, 3)
        WorldTestContext world = await CreateWorldAsync(width: 5, height: 3);

        // Scenario 1: Robot within world bounds
        await world.SendRobotAsync(x: 1, y: 1, orientation: "E", instructions: "RFRFRFRF")
            .ShouldExpectedlyResultIn(x: 1, y: 1, orientation: "E", lost: false);

        // Scenario 2: The robot flies off the edge of the net and leaves a scent
        await world.SendRobotAsync(x: 3, y: 2, orientation: "N", instructions: "FRRFLLFFRRFLL")
            .ShouldExpectedlyResultIn(x: 3, y: 3, orientation: "N", lost: true);

        // Scenario 3: The next robot smells the smell and ignores the dangerous command.
        await world.SendRobotAsync(x: 0, y: 3, orientation: "W", instructions: "LLFFFLFLFL")
            .ShouldExpectedlyResultIn(x: 2, y: 3, orientation: "S", lost: false);
    }

    private async Task<WorldTestContext> CreateWorldAsync(int width, int height)
    {
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("worlds", new { width, height });
        WorldResponse? worldDto = await response.Content.ReadFromJsonAsync<WorldResponse>();

        return new WorldTestContext(HttpClient, worldDto!.Id);
    }
}

public sealed class WorldTestContext(HttpClient httpClient, System.Guid worldId)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly System.Guid _worldId = worldId;

    public async Task<RobotTestAssertion> SendRobotAsync(int x, int y, string orientation, string instructions)
    {
        var request = new { x, y, orientation, instructions };
        
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync($"worlds/{_worldId}/robots", request);
        response.EnsureSuccessStatusCode();

        RobotResponse? result = await response.Content.ReadFromJsonAsync<RobotResponse>();
        return new RobotTestAssertion(result);
    }
}

public sealed class RobotTestAssertion(RobotResponse? result)
{
    private readonly RobotResponse _result = result ?? throw new System.ArgumentNullException(nameof(result));

    public RobotTestAssertion VerifyFields(int x, int y, string orientation, bool lost)
    {
        _result.X.ShouldBe(x);
        _result.Y.ShouldBe(y);
        _result.Orientation.ShouldBe(orientation);
        _result.Lost.ShouldBe(lost);
        return this;
    }
}

public static class RobotTestAssertionExtensions
{
    public static async Task<RobotTestAssertion> ShouldExpectedlyResultIn(
        this Task<RobotTestAssertion> assertionTask, 
        int x, 
        int y, 
        string orientation, 
        bool lost)
    {
        RobotTestAssertion assertion = await assertionTask;
        
        return assertion.VerifyFields(x, y, orientation, lost);
    }
}
