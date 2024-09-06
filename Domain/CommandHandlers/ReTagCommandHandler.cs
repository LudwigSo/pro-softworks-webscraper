using Domain.Ports;
using Domain.Ports.Queries;

namespace Domain.CommandHandlers
{
    public sealed record RetagCommand();

    public class RetagCommandHandler(ILogger logger, IProjectQueriesPort projectQueriesPort, ITagQueriesPort tagQueriesPort, IWriteContext writeContext)
    {
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IProjectQueriesPort _projectQueriesPort = projectQueriesPort ?? throw new ArgumentNullException(nameof(projectQueriesPort));
        private readonly ITagQueriesPort _tagQueriesPort = tagQueriesPort ?? throw new ArgumentNullException(nameof(tagQueriesPort));
        private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));

        public async Task Handle(RetagCommand command)
        {
            var projectCount = await _projectQueriesPort.GetProjectCount();
            _logger.LogInformation($"Retagging {projectCount} projects");
            var allTags = await _tagQueriesPort.GetAllTags();

            var page = 0;
            while (projectCount > 0)
            {
                var countToFetch = Math.Min(1000, projectCount);
                var projects = await _projectQueriesPort.GetAll(page, 1000, countToFetch);
                foreach (var project in projects)
                {
                    project.ReTag(allTags);
                }
                await _writeContext.SaveChangesAsync();

                projectCount -= countToFetch;
                _logger.LogInformation($"Retagged projects, skipped: {page * 1000}, took: {countToFetch}, remaining: {projectCount}");
                page++;
            }
            _logger.LogInformation("Retagged all projects");
        }
    }
}
