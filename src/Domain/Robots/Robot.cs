using SharedKernel;

namespace Domain.Robots;

/// <summary>
/// Represents a Mars Rover robot that can navigate and execute movement instructions on a grid world.
/// </summary>
/// <remarks>
/// This is an Aggregate Root/Entity within the Domain layer following Domain-Driven Design (DDD) principles.
/// </remarks>
public sealed class Robot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Robot"/> class with a private constructor.
    /// </summary>
    /// <param name="position">The initial coordinates of the robot on the grid.</param>
    /// <param name="orientation">The initial direction the robot is facing.</param>
    private Robot(RobotPosition position, RobotOrientation orientation)
    {
        Id = Guid.NewGuid();
        Position = position;
        Orientation = orientation;
        IsLost = false;
    }

    /// <summary>
    /// Gets the unique identifier of the robot.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the current coordinate position of the robot on the Mars grid.
    /// </summary>
    public RobotPosition Position { get; private set; }

    /// <summary>
    /// Gets the current orientation/direction (North, South, East, West) of the robot.
    /// </summary>
    public RobotOrientation Orientation { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the robot has moved off the grid edge and is permanently lost.
    /// </summary>
    public bool IsLost { get; private set; }

    /// <summary>
    /// Creates a new instance of a <see cref="Robot"/> with the specified position and orientation.
    /// </summary>
    /// <param name="position">The starting grid position.</param>
    /// <param name="orientation">The starting directional orientation.</param>
    /// <returns>A newly initialized <see cref="Robot"/> instance.</returns>
    public static Robot Create(RobotPosition position, RobotOrientation orientation) => new(position, orientation);

    /// <summary>
    /// Executes a rotation instruction for the robot. 
    /// </summary>
    /// <param name="instruction">The instruction to execute (Left or Right).</param>
    /// <remarks>
    /// If the robot is already lost, the instruction is ignored. 
    /// Forward movement instructions are evaluated outside this entity at the World level to check grid boundaries.
    /// </remarks>
    public void ExecuteInstruction(RobotInstruction instruction)
    {
        if (IsLost)
        {
            return;
        }

        switch (instruction)
        {
            case RobotInstruction.Left:
                TurnLeft();
                break;
            case RobotInstruction.Right:
                TurnRight();
                break;
            default:
                // Future instructions or Forward (handled at World level) are safely ignored
                break;
        }
    }

    /// <summary>
    /// Rotates the robot 90 degrees to the left (counter-clockwise) while remaining on the same grid point.
    /// </summary>
    public void TurnLeft()
    {
        Orientation = Orientation switch
        {
            RobotOrientation.N => RobotOrientation.W,
            RobotOrientation.W => RobotOrientation.S,
            RobotOrientation.S => RobotOrientation.E,
            RobotOrientation.E => RobotOrientation.N,
            _ => Orientation
        };
    }

    /// <summary>
    /// Rotates the robot 90 degrees to the right (clockwise) while remaining on the same grid point.
    /// </summary>
    public void TurnRight()
    {
        Orientation = Orientation switch
        {
            RobotOrientation.N => RobotOrientation.E,
            RobotOrientation.E => RobotOrientation.S,
            RobotOrientation.S => RobotOrientation.W,
            RobotOrientation.W => RobotOrientation.N,
            _ => Orientation
        };
    }

    /// <summary>
    /// Calculates and returns the next potential coordinate position if the robot were to move one grid point forward.
    /// </summary>
    /// <returns>A new <see cref="RobotPosition"/> representing the calculated coordinates ahead.</returns>
    public RobotPosition GetNextPosition()
    {
        return Orientation switch
        {
            RobotOrientation.N => Position with { Y = Position.Y + 1 },
            RobotOrientation.S => Position with { Y = Position.Y - 1 },
            RobotOrientation.E => Position with { X = Position.X + 1 },
            RobotOrientation.W => Position with { X = Position.X - 1 },
            _ => Position
        };
    }

    /// <summary>
    /// Updates the robot's location to the specified position.
    /// </summary>
    /// <param name="newPosition">The new grid coordinates to assign to the robot.</param>
    /// <remarks>
    /// If the robot is already marked as lost, this operation does nothing.
    /// </remarks>
    public void MoveTo(RobotPosition newPosition)
    {
        if (IsLost)
        {
            return;
        }

        Position = newPosition;
    }

    /// <summary>
    /// Marks the robot as permanently lost after moving off the bounded edge of the grid.
    /// </summary>
    public void MarkAsLost()
    {
        IsLost = true;
    }
}
