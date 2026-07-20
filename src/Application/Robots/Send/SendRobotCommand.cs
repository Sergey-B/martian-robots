using Application.Abstractions.Messaging;
using Domain.Robots;

namespace Application.Robots.Send;

/// <summary>
/// Represents the command to send a robot to a specific Mars world grid,
/// initialize its starting position and orientation, and execute a sequence of movement instructions.
/// </summary>
/// <param name="WorldId">The unique identifier of the target world grid where the robot will be deployed.</param>
/// <param name="X">The initial X coordinate position of the robot on the grid.</param>
/// <param name="Y">The initial Y coordinate position of the robot on the grid.</param>
/// <param name="Orientation">The initial direction string (e.g., "N", "S", "E", "W") the robot is facing.</param>
/// <param name="Instructions">The continuous string sequence of movement codes (composed of L, R, F) for the robot to execute.</param>
public sealed record SendRobotCommand(
    Guid WorldId, 
    int X, 
    int Y, 
    string Orientation, 
    string Instructions) : ICommand<RobotResponse>;
