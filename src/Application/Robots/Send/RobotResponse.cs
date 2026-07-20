using Domain.Robots;

namespace Application.Robots.Send;

public sealed record RobotResponse(int X, int Y, string Orientation, bool Lost);