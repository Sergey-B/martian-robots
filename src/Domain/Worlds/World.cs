using Domain.Robots;
using Domain.Scents;
using SharedKernel;

namespace Domain.Worlds;

/// <summary>
/// Represents the bounded Mars grid world that contains dimensions, manages robot instruction execution,
/// tracks scent trails of lost robots, and ensures future robots safely bypass dangerous boundaries.
/// </summary>
public sealed class World : Entity
{
    private const int MinDimension = 0;
    private const int MaxDimension = 50;

    private readonly List<Scent> _scents = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="World"/> class with a private constructor.
    /// </summary>
    /// <param name="width">The upper-right X coordinate constraint.</param>
    /// <param name="height">The upper-right Y coordinate constraint.</param>
    private World(int width, int height)
    {
        Id = Guid.NewGuid();
        Width = width;
        Height = height;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Required parameterless constructor for EF Core or other ORM/serialization frameworks.
    /// </summary>
    private World() { }

    /// <summary>
    /// Gets the unique identifier of this world.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the horizontal upper-bound coordinate boundary (Width).
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// Gets the vertical upper-bound coordinate boundary (Height).
    /// </summary>
    public int Height { get; private set; }

    /// <summary>
    /// Gets the timestamp when the world grid was created.
    /// </summary>
    public DateTime CreatedAt { get; private set; }
    
    /// <summary>
    /// Gets the read-only collection of scents dropped by lost robots on this world grid.
    /// </summary>
    public IReadOnlyCollection<Scent> Scents => _scents;

    /// <summary>
    /// Gets a value indicating whether the internal collection state has changed (e.g., a new scent was registered).
    /// </summary>
    public bool HasChanges { get; private set; }

    /// <summary>
    /// Factory method to securely instantiate a new <see cref="World"/> instance after applying domain constraints.
    /// </summary>
    /// <param name="height">The height value mapping to the top-right Y boundary.</param>
    /// <param name="width">The width value mapping to the top-right X boundary.</param>
    /// <returns>A successful <see cref="Result{World}"/> object, or a validation failure.</returns>
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
    
    /// <summary>
    /// Progresses a robot step-by-step through a continuous instruction sequence on this grid system.
    /// </summary>
    /// <param name="robot">The robot processing instructions.</param>
    /// <param name="instructions">The string chain composed of structural operational codes (L, R, F).</param>
    /// <returns>A structural domain <see cref="Result{RobotResult}"/> reflecting the absolute final displacement metrics.</returns>
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
        } // <- Fixed syntax error: Added missing closing bracket for the foreach loop

        return Result.Success(new RobotResult(robot.Position, robot.Orientation, robot.IsLost));
    }
    
    /// <summary>
    /// Evaluates whether specific evaluation coordinates exist beyond the grid parameters.
    /// </summary>
    private bool IsOutOfBounds(RobotPosition position) =>
        position.X < MinDimension || position.X > Width || position.Y < MinDimension || position.Y > Height;

    /// <summary>
    /// Adds a scent marker to tracking records and shifts the modified state tracking variable.
    /// </summary>
    private void AddScent(RobotPosition position) 
    {
        _scents.Add(Scent.Create(worldId: Id, x: position.X, y: position.Y));
        HasChanges = true;
    }
        
    /// <summary>
    /// Verifies if a scent signature exists within grid coordinates to save subsequent rovers.
    /// </summary>
    private bool HasScentAt(RobotPosition position) =>
        _scents.Any(s => s.X == position.X && s.Y == position.Y);
}
