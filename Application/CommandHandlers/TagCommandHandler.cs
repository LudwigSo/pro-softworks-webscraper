using Application.Ports;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Application.CommandHandlers;

public record CreateTagCommand(string Name);
public record DeleteTagCommand(int Id);
public record RetagCommand();

public class TagCommandHandler(ILogger logger, Context dbContext)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Context _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task Handle(CreateTagCommand command)
    {
        var tag = new Tag(command.Name);
        await _dbContext.Tags.AddAsync(tag);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Added tag {tag.Id}:{tag.Name}");
    }

    public async Task Handle(DeleteTagCommand command)
    {
        var tagToDelete = await _dbContext.Tags.FindAsync(command.Id);
        if (tagToDelete is null)
        {
            _logger.LogInformation($"Tag cannot be deleted because it does not exist: {command.Id}");
            return;
        }

        _dbContext.Tags.Remove(tagToDelete);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Deleted tag {tagToDelete.Id}:{tagToDelete.Name}");
    }

    public async Task Handle(RetagCommand command)
    {
        var projectCount = await _dbContext.Projects.CountAsync();
        _logger.LogInformation($"Retagging {projectCount} projects");
        var allTags = await _dbContext.Tags.ToArrayAsync();

        var page = 0;
        while (projectCount > 0)
        {
            var countToFetch = Math.Min(1000, projectCount);
            var projects = await _dbContext.Projects.Include(p => p.Tags).OrderBy(p => p.Id).Skip(page * 1000).Take(countToFetch).ToArrayAsync();
            foreach (var project in projects)
            {
                project.ReTag(allTags);
            }
            await _dbContext.SaveChangesAsync();

            projectCount -= countToFetch;
            _logger.LogInformation($"Retagged projects, skipped: {page * 1000}, took: {countToFetch}, remaining: {projectCount}");
            page++;
        }
        _logger.LogInformation("Retagged all projects");
    }
}
