using Application.Abstractions.Messaging;

namespace Application.Worlds.Create;

/// <summary>
/// Represents the command to initialize and create a new bounded rectangular Mars world grid 
/// with specific horizontal and vertical dimensions.
/// </summary>
/// <param name="Width">The upper-right horizontal grid coordinate boundary constraint (Max 50).</param>
/// <param name="Height">The upper-right vertical grid coordinate boundary constraint (Max 50).</param>
public sealed record CreateWorldCommand(int Width, int Height) : ICommand<Guid>;
