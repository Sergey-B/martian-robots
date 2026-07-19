using Application.Abstractions.Messaging;
using Application.Worlds.Create;
using Domain.Worlds;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds;

internal sealed class Create : IEndpoint
{
    public sealed record CreateWorldRequest(int Width, int Height);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("worlds", async (
            CreateWorldRequest request,
            ICommandHandler<CreateWorldCommand, Guid> handler,
            CancellationToken cancellationToken) =>
        {
            var command = new CreateWorldCommand(request.Width, request.Height);
            
            Result<Guid> result = await handler.Handle(command, cancellationToken);

            return result.Match(Results.Created, CustomResults.Problem);
        })
        .WithTags(Tags.Worlds);
    }
}
