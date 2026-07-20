using Application.Abstractions.Messaging;
using Application.Robots.Send;
using Domain.Robots;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds.Robots;

internal sealed class Send : IEndpoint
{
    public sealed record Request
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Orientation { get; set; }
        public string Instructions { get; set; }
    }

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
