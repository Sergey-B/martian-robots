using Application.Abstractions.Messaging;
using Application.Worlds.Get;
using SharedKernel;
using Web.Api.Extensions;
using Web.Api.Infrastructure;

namespace Web.Api.Endpoints.Worlds;

internal sealed class Get : IEndpoint
{
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
