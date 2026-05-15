using NetworkTopologyVisitingCard.Data;
using NetworkTopologyVisitingCard.Models;

namespace NetworkTopologyVisitingCard.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Projects = new Repository<Project>(context);
        Reviews = new Repository<Review>(context);
        Technologies = new Repository<Technology>(context);
        NetworkTopologies = new Repository<NetworkTopology>(context);
    }

    public IRepository<Project> Projects { get; }
    public IRepository<Review> Reviews { get; }
    public IRepository<Technology> Technologies { get; }
    public IRepository<NetworkTopology> NetworkTopologies { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public void Dispose()
    {
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
