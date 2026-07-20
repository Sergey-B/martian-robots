using SharedKernel;

namespace Domain.Robots;

public static class RobotErrors
{
    public static readonly Error InvalidOrientation = Error.Problem(
        "Robot.InvalidOrientation",
        "The robot has invalid orientation.");
}
