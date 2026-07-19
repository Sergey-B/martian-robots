using Domain.Scents;
using Domain.Worlds;
using Microsoft.EntityFrameworkCore;

namespace Application.Abstractions.Data;

public interface IApplicationDbContext
{
    DbSet<World> Worlds { get; }
    DbSet<Scent> Scents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
