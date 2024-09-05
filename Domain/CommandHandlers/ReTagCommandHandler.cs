using Domain.Ports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CommandHandlers
{
    public sealed record ReTagCommand();

    public class ReTagCommandHandler(ILogger logger, IProjectQueriesPort projectQueriesPort, IWriteContext writeContext)
    {
        private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly IProjectQueriesPort _projectQueriesPort = projectQueriesPort ?? throw new ArgumentNullException(nameof(projectQueriesPort));
        private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));

        public async Task Handle(ReTagCommand command)
        {
            var projectCount = await _projectQueriesPort.GetProjectCount();
            _logger.LogInformation($"Retagging {projectCount} projects");
            var allTags = await _projectQueriesPort.GetAllTags();

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
