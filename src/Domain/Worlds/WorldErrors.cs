using SharedKernel;

namespace Domain.Worlds;

public static class WorldErrors
{
    public static Error NotFound(Guid worldId) => Error.NotFound(
       "Worlds.NotFound",
       $"The world with the Id = '{worldId}' was not found");

    public static readonly Error NotValidCoordinates = Error.Problem(
        "World.NotValid",
        "The world is not valid, coordinates should be in between 0 and 50.");

    public static readonly Error InvalidWidth = Error.Problem(
        "World.InvalidWidth",
        "The world width is not valid, width should be in between 0 and 50.");

    public static readonly Error InvalidHeight = Error.Problem(
        "World.InvalidHeight",
        "The world height is not valid, width should be in between 0 and 50.");
        
    public static readonly Error RobotSpawnOutOfBounds = Error.Problem(
        "World.RobotSpawnOutOfBounds",
        "The robot position is out of world dimensions.");
}
