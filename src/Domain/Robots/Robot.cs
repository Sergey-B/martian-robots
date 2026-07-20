using SharedKernel;

namespace Domain.Robots;

public sealed class Robot
{
    private Robot(RobotPosition position, RobotOrientation orientation)
    {
        Id = Guid.NewGuid();
        Position = position;
        Orientation = orientation;
        IsLost = false;
    }

    public Guid Id { get; private set; }
    public RobotPosition Position { get; private set; }
    public RobotOrientation Orientation { get; private set; }
    public bool IsLost { get; private set; }

    public static Robot Create(RobotPosition position, RobotOrientation orientation) => new(position, orientation);

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
    /// Возвращает позицию, в которую переместится робот, если сделает шаг вперед.
    /// </summary>
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

    public void MoveTo(RobotPosition newPosition)
    {
        if (IsLost)
        {
            return;
        }

        Position = newPosition;
    }

    public void MarkAsLost()
    {
        IsLost = true;
    }
}
