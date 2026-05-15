using NetworkTopologyVisitingCard.Models;

namespace NetworkTopologyVisitingCard.Repositories;

public interface IUnitOfWork : IDisposable
{
    IRepository<Project> Projects { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Technology> Technologies { get; }
    IRepository<NetworkTopology> NetworkTopologies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
