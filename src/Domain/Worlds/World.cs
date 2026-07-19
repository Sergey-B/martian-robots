using Domain.Robots;
using Domain.Scents;
using SharedKernel;

namespace Domain.Worlds;

public sealed class World : Entity
{
    private const int MinDimension = 0;
    private const int MaxDimension = 50;

    private readonly List<Scent> _scents = new();

    private World(int width, int height)
    {
        Id = Guid.NewGuid();
        Width = width;
        Height = height;
        CreatedAt = DateTime.UtcNow;
    }

    private World() { }

    public Guid Id { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public DateTime CreatedAt { get; private set; }
    
    public IReadOnlyCollection<Scent> Scents => _scents;

    public bool HasChanges { get; private set; }

    public static Result<World> Create(int height, int width)
    {
        if (width is < MinDimension or > MaxDimension)
        {
            return Result.Failure<World>(WorldErrors.InvalidWidth);
        }

        if (height is < MinDimension or > MaxDimension)
        {
            return Result.Failure<World>(WorldErrors.InvalidHeight);
        }

        return Result.Success(new World(width, height));
    }
    
    public Result<RobotResult> ProcessRobotInstructions(Robot robot, string instructions)
    {
        if (IsOutOfBounds(robot.Position))
        {
            return Result.Failure<RobotResult>(WorldErrors.RobotSpawnOutOfBounds);
        }

        foreach (char cmd in instructions)
        {
            if (robot.IsLost)
            {
                break;
            }

            // Cast char directly to our strongly-typed enum
            var instruction = (RobotInstruction)cmd;

            if (instruction is RobotInstruction.Left or RobotInstruction.Right)
            {
                robot.ExecuteInstruction(instruction);
            }
            else if (instruction is RobotInstruction.Forward)
            {
                RobotPosition nextPosition = robot.GetNextPosition();
                
                if (IsOutOfBounds(nextPosition))
                {
                    if (HasScentAt(robot.Position))
                    {
                        continue; 
                    }

                    AddScent(robot.Position);
                    robot.MarkAsLost();
                }
                else
                {
                    robot.MoveTo(nextPosition);
                }
            }
            else
            {
                // Provision for additional command types in the future.
                robot.ExecuteInstruction(instruction);
            }
    }

    return Result.Success(new RobotResult(robot.Position, robot.Orientation, robot.IsLost));
    }
    
    private bool IsOutOfBounds(RobotPosition position) =>
        position.X < MinDimension || position.X > Width || position.Y < MinDimension || position.Y > Height;

    private void AddScent(RobotPosition position) {
        _scents.Add(Scent.Create(worldId: Id, x: position.X, y: position.Y));

        HasChanges = true;
    }
        
    private bool HasScentAt(RobotPosition position) =>
        _scents.Any(s => s.X == position.X && s.Y == position.Y);
}
