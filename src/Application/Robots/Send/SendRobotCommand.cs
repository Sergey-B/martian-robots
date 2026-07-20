using Application.Abstractions.Messaging;
using Domain.Robots;

namespace Application.Robots.Send;

public sealed record SendRobotCommand(Guid WorldId, int X, int Y, string Orientation, string Instructions) : ICommand<RobotResponse>;
