using Domain;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres;

public class Context : DbContext
{
    public Context() { }
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Project> Projects { get; init; }
    public DbSet<Tag> Tags { get; init; }
    public DbSet<Keyword> Keywords { get; init; }


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
            entity.Property(e => e.ClaimedBy).HasMaxLength(100);
            entity.Property(e => e.PlannedStart).HasColumnType("timestamp");
            entity.Property(e => e.PostedAt).HasColumnType("timestamp");
            entity.Property(e => e.FirstSeenAt).HasColumnType("timestamp").IsRequired();

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
            entity.HasMany(e => e.Keywords).WithOne().HasForeignKey(k => k.TagId);
        });

        modelBuilder.Entity<Tag>().HasData(
            new Tag("C#")           { Id = 1},
            new Tag("Architecture") { Id = 2},
            new Tag("UI")           { Id = 3}
        );

        modelBuilder.Entity<Keyword>(entity =>
        {
            entity.ToTable("Keyword");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Value).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<Keyword>().HasData(
            new Keyword(1, "c#")       { Id = 1 },
            new Keyword(1, "csharp")   { Id = 2 },
            new Keyword(1, "c-sharp")  { Id = 3 },
            new Keyword(1, "aspnet")   { Id = 4 },
            new Keyword(1, "dotnet")   { Id = 5 },

            new Keyword(2, "ddd")                  { Id = 6 },
            new Keyword(2, "domaindrivendesign")   { Id = 7 },
            new Keyword(2, "domain-driven-design") { Id = 8 },
            new Keyword(2, "eric evans")           { Id = 9 },
            new Keyword(2, "ericevans")            { Id = 10 },
            new Keyword(2, "eric-evans")           { Id = 11 },
            new Keyword(2, "portsandadapters")     { Id = 12 },
            new Keyword(2, "ports&adapters")       { Id = 13 },
            new Keyword(2, "ports-and-adapters")   { Id = 14 },
            new Keyword(2, "hexagonal")            { Id = 15 },
            new Keyword(2, "clean")                { Id = 16 },
            new Keyword(2, "layered")              { Id = 17 },

            new Keyword(3, "vuejs")        { Id = 18 },
            new Keyword(3, "vue.js")       { Id = 19 },
            new Keyword(3, "vuetify")      { Id = 20 },
            new Keyword(3, "javascript")   { Id = 21 },
            new Keyword(3, "typscript")    { Id = 22 },
            new Keyword(3, "figma")        { Id = 23 }
        );
    }
}
