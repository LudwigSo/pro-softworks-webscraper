using Domain;
using Microsoft.EntityFrameworkCore;

namespace Driven.Persistence.Postgres;

public class Context : DbContext
{
    public Context() { }
    public Context(DbContextOptions<Context> options) : base(options) { }

    public DbSet<Project> Projects { get; init; }
    public DbSet<Tag> Tags { get; init; }
    public DbSet<Tag> Keywords { get; init; }


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
            new Keyword("c#")       { Id = 1, TagId = 1 },
            new Keyword("csharp")   { Id = 2, TagId = 1 },
            new Keyword("c-sharp")  { Id = 3, TagId = 1 },
            new Keyword("aspnet")   { Id = 4, TagId = 1 },
            new Keyword("dotnet")   { Id = 5, TagId = 1 },

            new Keyword("ddd")                  { Id = 6, TagId = 2 },
            new Keyword("domaindrivendesign")   { Id = 7, TagId = 2 },
            new Keyword("domain-driven-design") { Id = 8, TagId = 2 },
            new Keyword("eric evans")           { Id = 9, TagId = 2 },
            new Keyword("ericevans")            { Id = 10, TagId = 2 },
            new Keyword("eric-evans")           { Id = 11, TagId = 2 },
            new Keyword("portsandadapters")     { Id = 12, TagId = 2 },
            new Keyword("ports&adapters")       { Id = 13, TagId = 2 },
            new Keyword("ports-and-adapters")   { Id = 14, TagId = 2 },
            new Keyword("hexagonal")            { Id = 15, TagId = 2 },
            new Keyword("clean")                { Id = 16, TagId = 2 },
            new Keyword("layered")              { Id = 17, TagId = 2 },

            new Keyword("vuejs")        { Id = 18, TagId = 3 },
            new Keyword("vue.js")       { Id = 19, TagId = 3 },
            new Keyword("vuetify")      { Id = 20, TagId = 3 },
            new Keyword("javascript")   { Id = 21, TagId = 3 },
            new Keyword("typscript")    { Id = 22, TagId = 3 },
            new Keyword("figma")        { Id = 23, TagId = 3 }
        );
    }
}
