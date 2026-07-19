using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Worlds.Get;

internal sealed class GetWorldsQueryHandler(IApplicationDbContext context) : IQueryHandler<GetWorldsQuery, List<WorldResponse>>
{
    public async Task<Result<List<WorldResponse>>> Handle(GetWorldsQuery query, CancellationToken cancellationToken)
    {
        List<WorldResponse> worlds = await context.Worlds
            .Select(world => new WorldResponse(Id: world.Id, Width: world.Width, Height: world.Height))
            .ToListAsync(cancellationToken);

        return worlds;
    }
}
