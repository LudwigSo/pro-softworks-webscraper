using Driven.Persistence.Postgres;
using Microsoft.Extensions.Logging;

namespace Application.CommandHandlers;

public record ClaimProjectCommand(int ProjectId, string Username);
public class ClaimProjectCommandHandler(ILogger<ClaimProjectCommandHandler> logger, Context dbContext)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Context _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task Handle(ClaimProjectCommand command)
    {
        var project = await _dbContext.Projects.FindAsync(command.ProjectId);
        if (project is null) throw new InvalidOperationException($"Project {command.ProjectId} does not exist");

        project.Claim(command.Username);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Claimed project {project.Id} by {command.Username}");
    }
}
