namespace Application.Worlds;

/// <summary>
/// Represents the data transfer object containing the identification and dimension metrics 
/// of a retrieved Mars world grid configuration.
/// </summary>
/// <param name="Id">The unique identifier of the stored world grid.</param>
/// <param name="Width">The upper-right horizontal grid coordinate boundary (Width).</param>
/// <param name="Height">The upper-right vertical grid coordinate boundary (Height).</param>
public sealed record WorldResponse(Guid Id, int Width, int Height);
