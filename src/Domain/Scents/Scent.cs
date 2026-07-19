using SharedKernel;

namespace Domain.Scents;

public sealed class Scent : Entity
{
    private Scent(Guid worldId, int x, int y)
    {
        Id = Guid.NewGuid();
        WorldId = worldId;
        X = x;
        Y = y;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public Guid WorldId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    internal static Scent Create(Guid worldId, int x, int y) => new(worldId, x, y);
}
