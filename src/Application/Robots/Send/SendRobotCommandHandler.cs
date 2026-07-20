using Application.Abstractions.Authentication;
using Application.Abstractions.Data;
using Application.Abstractions.Messaging;
using Domain.Robots;
using Domain.Scents;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;
using SharedKernel;

namespace Application.Robots.Send;

internal sealed class SendRobotCommandHandler(
    IApplicationDbContext context)
    : ICommandHandler<SendRobotCommand, RobotResponse>
{
    public async Task<Result<RobotResponse>> Handle(SendRobotCommand command, CancellationToken cancellationToken)
    {
        // get world
        World? world = await context.Worlds
            .Include(w => w.Scents)
            .FirstOrDefaultAsync(w => w.Id == command.WorldId , cancellationToken);
        if (world == null)
        {
            return Result.Failure<RobotResponse>(WorldErrors.NotFound(command.WorldId));
        }

        // build robot
        if (!Enum.TryParse(command.Orientation, out RobotOrientation orientation))
        {
            return Result.Failure<RobotResponse>(RobotErrors.InvalidOrientation);
        }
        Result<Robot> robotCreateResult = Robot.Create(new RobotPosition(command.X, command.Y), orientation);

        if (robotCreateResult.IsFailure)
        {
            return Result.Failure<RobotResponse>(robotCreateResult.Error);
        }

        Robot robot = robotCreateResult.Value;
        
        // process robot instructions
        Guid[] existingScentIds = [.. world.Scents.Select(s => s.Id)];
        Result<RobotResult> processResult = world.ProcessRobotInstructions(robot, command.Instructions);
        if (processResult.IsFailure)
        {
            return Result.Failure<RobotResponse>(processResult.Error);
        }

        if (world.HasChanges)
        {
            List<Scent> newScents = [.. world.Scents.Where(s => !existingScentIds.Contains(s.Id))]; // can be a method inside world 
            context.Scents.AddRange(newScents);

            await context.SaveChangesAsync(cancellationToken);
        }

        return new RobotResponse(
            processResult.Value.Position.X,
            processResult.Value.Position.Y,
            processResult.Value.Orientation.ToString(),
            processResult.Value.Lost);
    }
}
