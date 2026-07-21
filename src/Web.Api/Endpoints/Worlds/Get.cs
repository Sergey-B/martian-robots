using Application.Abstractions.Messaging;
using Application.Worlds;
using Application.Worlds.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds;

/// <summary>
/// Represents the Minimal API endpoint for retrieving a list of all existing Mars world grids.
/// </summary>
internal sealed class Get : IEndpoint
{
    /// <summary>
    /// Maps the world grid retrieval query route into the application's request pipeline configuration.
    /// </summary>
    /// <param name="app">The endpoint route builder infrastructure instance.</param>
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("worlds", async (
            IQueryHandler<GetWorldsQuery, List<WorldResponse>> handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetWorldsQuery();

            Result<List<WorldResponse>> result = await handler.Handle(query, cancellationToken);

            return result.Match(Results.Ok, CustomResults.Problem);
        })
        .WithTags(Tags.Worlds);
    }
}
