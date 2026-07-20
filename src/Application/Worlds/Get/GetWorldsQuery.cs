using Application.Abstractions.Messaging;

namespace Application.Worlds.Get;

/// <summary>
/// Represents the query to retrieve a collection of all stored bounded Mars world grid configurations.
/// </summary>
/// <remarks>
/// This query implements <see cref="IQuery{T}"/> and returns a read-only list of <see cref="WorldResponse"/> DTOs 
/// representing the existing dimensions and tracking configurations.
/// </remarks>
public sealed record GetWorldsQuery() : IQuery<List<WorldResponse>>;
