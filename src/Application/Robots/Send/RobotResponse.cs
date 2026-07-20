using Domain.Robots;

namespace Application.Robots.Send;

/// <summary>
/// Represents the data transfer object returned after a robot finishes executing its instructions,
/// containing its final operational positioning metrics and lifecycle safety state.
/// </summary>
/// <param name="X">The final X coordinate position of the robot on the grid.</param>
/// <param name="Y">The final Y coordinate position of the robot on the grid.</param>
/// <param name="Orientation">The final direction string (e.g., "N", "S", "E", "W") the robot is facing.</param>
/// <param name="Lost">A value indicating whether the robot moved off the bounded grid edge and was lost.</param>
public sealed record RobotResponse(
    int X, 
    int Y, 
    string Orientation, 
    bool Lost);
