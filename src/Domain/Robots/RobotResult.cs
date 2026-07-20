namespace Domain.Robots;

public sealed record RobotResult(RobotPosition Position, RobotOrientation Orientation, bool Lost);
