using Application.Abstractions.Messaging;
using Application.Worlds.Create;
using Domain.Worlds;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds;

/// <summary>
/// Represents the Minimal API endpoint for creating a new bounded rectangular Mars world grid 
/// or retrieving an existing one with identical dimensions.
/// </summary>
internal sealed class Create : IEndpoint
{
    /// <summary>
    /// Represents the HTTP request payload specifying the required dimensions for the new world grid.
    /// </summary>
    /// <param name="Width">The upper-right horizontal grid coordinate boundary constraint.</param>
    /// <param name="Height">The upper-right vertical grid coordinate boundary constraint.</param>
    public sealed record CreateWorldRequest(int Width, int Height);

    /// <summary>
    /// Maps the world grid creation route into the application's request pipeline configuration.
    /// </summary>
    /// <param name="app">The endpoint route builder infrastructure instance.</param>
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
