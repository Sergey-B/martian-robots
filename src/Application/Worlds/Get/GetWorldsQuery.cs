using Application.Abstractions.Messaging;

namespace Application.Worlds.Get;

public sealed record GetWorldsQuery() : IQuery<List<WorldResponse>>;
