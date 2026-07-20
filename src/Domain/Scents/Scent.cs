using SharedKernel;

namespace Domain.Scents;

/// <summary>
/// Represents a scent mark left on a grid coordinate by a lost robot.
/// Prevents subsequent robots from falling off the world grid at the same location.
/// </summary>
public sealed class Scent : Entity
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Scent"/> class with a private constructor.
    /// </summary>
    /// <param name="worldId">The unique identifier of the world grid where the scent is left.</param>
    /// <param name="x">The X coordinate of the last safe grid position.</param>
    /// <param name="y">The Y coordinate of the last safe grid position.</param>
    private Scent(Guid worldId, int x, int y)
    {
        Id = Guid.NewGuid();
        WorldId = worldId;
        X = x;
        Y = y;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Required parameterless constructor for EF Core or other ORM/serialization frameworks.
    /// </summary>
    private Scent() { }

    /// <summary>
    /// Gets the unique identifier of the scent record.
    /// </summary>
    public Guid Id { get; private set; }

    /// <summary>
    /// Gets the X coordinate of the grid position where the scent was left.
    /// </summary>
    public int X { get; private set; }

    /// <summary>
    /// Gets the Y coordinate of the grid position where the scent was left.
    /// </summary>
    public int Y { get; private set; }

    /// <summary>
    /// Gets the unique identifier of the world grid associated with this scent.
    /// </summary>
    public Guid WorldId { get; private set; }

    /// <summary>
    /// Gets the timestamp when the scent was registered.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Factory method used internally by the domain layer (typically from the <c>World</c> aggregate root) 
    /// to instantiate a new scent marker.
    /// </summary>
    /// <param name="worldId">The target world identifier.</param>
    /// <param name="x">The safe boundary X coordinate.</param>
    /// <param name="y">The safe boundary Y coordinate.</param>
    /// <returns>A newly initialized <see cref="Scent"/> instance.</returns>
    internal static Scent Create(Guid worldId, int x, int y) => new(worldId, x, y);
}
