using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Application.Worlds.Get;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Worlds.Create;

/// <summary>
/// Handles the execution of a <see cref="CreateWorldCommand"/> by validating grid constraints,
/// checking for existing duplicates, and persisting the new world matrix configuration.
/// </summary>
internal sealed class CreateWorldCommandHandler(IApplicationDbContext context) 
    : ICommandHandler<CreateWorldCommand, WorldResponse>
{
    /// <summary>
    /// Processes the incoming request to define and store a new or existing bounded Mars world grid.
    /// </summary>
    /// <param name="command">The command specifications containing the requested height and width dimensions.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A successful <see cref="Result{Guid}"/> returning the unique ID of the world grid, or a functional domain error.</returns>
    public async Task<Result<WorldResponse>> Handle(CreateWorldCommand command, CancellationToken cancellationToken)
    {
        // 1. Check if a world with the exact same dimensions already exists to prevent duplicate entities
        World? world = await context.Worlds.FirstOrDefaultAsync(w =>
            w.Height == command.Height
            && w.Width == command.Width,
            cancellationToken);
        
        if (world != null)
        {
            return new WorldResponse(
            world.Id,
            world.Width,
            world.Height);
        }

        // 2. Execute factory validation rules inside the Domain Layer before grid instantiation
        Result<World> result = World.Create(height: command.Height, width: command.Width);
        if (result.IsFailure)
        {
            return Result.Failure<WorldResponse>(result.Error);
        }

        // 3. Track and persist the newly formed aggregate state inside the infrastructure layer tables
        await context.Worlds.AddAsync(result.Value, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        // 4. Return the newly registered entity's identifier tracking sequence
        return new WorldResponse(
            result.Value.Id,
            result.Value.Width,
            result.Value.Height);
    }
}
