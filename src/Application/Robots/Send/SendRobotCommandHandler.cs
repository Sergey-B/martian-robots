using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Robots;
using Domain.Scents;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Robots.Send;

/// <summary>
/// Handles the execution of a <see cref="SendRobotCommand"/> by loading the specified world grid,
/// initializing a new robot entity, simulating its movements, and persisting any new scent trails.
/// </summary>
internal sealed class SendRobotCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<SendRobotCommand, RobotResponse>
{
    /// <summary>
    /// Processes the incoming command to simulate and track a robot's execution sequence on Mars.
    /// </summary>
    /// <param name="command">The command containing world ID, initial positioning, orientation, and instruction strings.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A successful <see cref="Result{RobotResponse}"/> with the final state metrics, or a functional domain error.</returns>
    public async Task<Result<RobotResponse>> Handle(SendRobotCommand command, CancellationToken cancellationToken)
    {
        // 1. Retrieve the target world grid including its registered scents for boundary analysis
        World? world = await context.Worlds
            .Include(w => w.Scents)
            .FirstOrDefaultAsync(w => w.Id == command.WorldId, cancellationToken);
            
        if (world == null)
        {
            return Result.Failure<RobotResponse>(WorldErrors.NotFound(command.WorldId));
        }

        // 2. Validate and map the raw input string orientation to the strong domain enum
        if (!Enum.TryParse(command.Orientation, out RobotOrientation orientation))
        {
            return Result.Failure<RobotResponse>(RobotErrors.InvalidOrientation);
        }

        // 3. Initialize the robot instance (Robot.Create directly returns a Robot object, not a Result<Robot>)
        var robot = Robot.Create(new RobotPosition(command.X, command.Y), orientation);
        
        // 4. Track existing scents prior to command execution to identify newly registered scents afterwards
        Guid[] existingScentIds = [.. world.Scents.Select(s => s.Id)];
        
        // 5. Simulate execution workflow rules across the active coordinates grid system
        Result<RobotResult> processResult = world.ProcessRobotInstructions(robot, command.Instructions);
        if (processResult.IsFailure)
        {
            return Result.Failure<RobotResponse>(processResult.Error);
        }

        // 6. Conditionally extract and append newly registered scent trails to tracking tables
        if (world.HasChanges)
        {
            List<Scent> newScents = [.. world.Scents.Where(s => !existingScentIds.Contains(s.Id))];
            context.Scents.AddRange(newScents);

            await context.SaveChangesAsync(cancellationToken);
        }

        // 7. Map the result data metrics transfer parameters back to presenting api controllers
        return new RobotResponse(
            processResult.Value.Position.X,
            processResult.Value.Position.Y,
            processResult.Value.Orientation.ToString(),
            processResult.Value.Lost);
    }
}
