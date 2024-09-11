using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres;

public class Context : DbContext
{
    public Context() { }
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Project> Projects { get; init; }
    public DbSet<Tag> Tags { get; init; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Title).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Url).IsRequired();
            entity.Property(e => e.Source).IsRequired();
            entity.Property(e => e.ProjectIdentifier).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(5000);
            entity.Property(e => e.JobLocation).HasMaxLength(200);
            entity.Property(e => e.PlannedStart).HasColumnType("timestamp");
            entity.Property(e => e.PostedAt).HasColumnType("timestamp");
            entity.Property(e => e.FirstSeenAt).HasColumnType("timestamp").IsRequired();
            entity.Property(e => e.RemovedAt).HasColumnType("timestamp");

            entity.HasMany(e => e.Tags)
                .WithMany()
                .UsingEntity(
                    "Project_Tag",
                    l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId"),
                    r => r.HasOne(typeof(Project)).WithMany().HasForeignKey("ProjectId")
            );
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.ToTable("Tag");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name).HasMaxLength(50).IsRequired();
        });
    }
}
