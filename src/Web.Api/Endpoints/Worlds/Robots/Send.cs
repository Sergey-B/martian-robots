using Application.Abstractions.Messaging;
using Application.Robots.Send;
using Domain.Robots;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds.Robots;

/// <summary>
/// Represents the Minimal API endpoint for deploying a robot and processing its movement instructions 
/// within a specific Mars world grid.
/// </summary>
internal sealed class Send : IEndpoint
{
    public sealed record Request
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Orientation { get; set; }
        public string Instructions { get; set; }
    }

    /// <summary>
    /// Maps the robot instruction processing route into the application's request pipeline configuration.
    /// </summary>
    /// <param name="app">The endpoint route builder infrastructure instance.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("worlds/{worldId:guid}/robots", async (
            Guid worldId,
            Request request,
            ICommandHandler<SendRobotCommand, RobotResponse> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new SendRobotCommand(
                WorldId: worldId,
                X: request.X,
                Y: request.Y,
                Orientation: request.Orientation,
                Instructions: request.Instructions);
          
            Result<RobotResponse> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Robots);
    }
}
