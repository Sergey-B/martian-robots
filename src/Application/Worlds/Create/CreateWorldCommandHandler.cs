using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Worlds.Create;

internal sealed class CreateWorldCommandCommandHandler(IApplicationDbContext context) : ICommandHandler<CreateWorldCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateWorldCommand command, CancellationToken cancellationToken)
    {
        World? world = await context.Worlds.FirstOrDefaultAsync(w =>
            w.Height == command.Height
            && w.Width == command.Width,
            cancellationToken);
        
        if (world != null)
        {
            return world.Id;
        }

        Result<World> result = World.Create(height: command.Height, width: command.Width);
        if (result.IsFailure)
        {
            return Result.Failure<Guid>(result.Error);
        }

        await context.Worlds.AddAsync(result.Value, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return result.Value.Id;
    }
}
