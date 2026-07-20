using Application.Abstractions.Messaging;

namespace Application.Worlds.Create;

public sealed record CreateWorldCommand(int Width, int Height) : ICommand<Guid>;