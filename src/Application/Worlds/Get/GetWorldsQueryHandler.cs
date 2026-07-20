using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Worlds.Get;

/// <summary>
/// Handles the execution of a <see cref="GetWorldsQuery"/> by querying the database,
/// data projection mapping, and retrieving a collection of all stored world grid configurations.
/// </summary>
internal sealed class GetWorldsQueryHandler(IApplicationDbContext context) 
    : IQueryHandler<GetWorldsQuery, List<WorldResponse>>
{
    /// <summary>
    /// Processes the incoming query request to fetch and present existing defined bounded Mars worlds.
    /// </summary>
    /// <param name="query">The query parameters request instance.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A successful <see cref="Result{T}"/> containing a projection list of <see cref="WorldResponse"/> mappings.</returns>
    public async Task<Result<List<WorldResponse>>> Handle(GetWorldsQuery query, CancellationToken cancellationToken)
    {
        // 1. Project data metrics collection directly via EF Core to optimize memory allocation performance
        List<WorldResponse> worlds = await context.Worlds
            .Select(world => new WorldResponse(world.Id, world.Width, world.Height))
            .ToListAsync(cancellationToken);

        // 2. Wrap and return encapsulated collection response projection state tracking parameters
        return Result.Success(worlds);
    }
}
