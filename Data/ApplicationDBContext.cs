using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NetworkTopologyVisitingCard.Models;

namespace NetworkTopologyVisitingCard.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Technology> Technologies => Set<Technology>();
    public DbSet<NetworkTopology> NetworkTopologies => Set<NetworkTopology>();
    public DbSet<ProjectTechnology> ProjectTechnologies => Set<ProjectTechnology>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Review>()
            .HasOne(r => r.Project)
            .WithMany(p => p.Reviews)
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<NetworkTopology>()
            .HasOne(t => t.Project)
            .WithMany(p => p.NetworkTopologies)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectTechnology>()
            .HasKey(pt => new { pt.ProjectId, pt.TechnologyId });

        builder.Entity<ProjectTechnology>()
            .HasOne(pt => pt.Project)
            .WithMany(p => p.ProjectTechnologies)
            .HasForeignKey(pt => pt.ProjectId);

        builder.Entity<ProjectTechnology>()
            .HasOne(pt => pt.Technology)
            .WithMany(t => t.ProjectTechnologies)
            .HasForeignKey(pt => pt.TechnologyId);

        builder.Entity<Project>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.Projects)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
