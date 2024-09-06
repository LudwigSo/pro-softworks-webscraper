using Domain.CommandHandlers;
using Domain.Model;
using Domain.Ports.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]/")]
    public class ProjectController(
        IProjectQueriesPort projectQueries, 
        RetagCommandHandler retagCommandHandler
    ) : ControllerBase
    {
        private readonly IProjectQueriesPort _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
        private readonly RetagCommandHandler _retagCommandHandler = retagCommandHandler ?? throw new ArgumentNullException(nameof(retagCommandHandler));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Project>> AllActiveWithAnyTag() => await _projectQueries.GetActiveWithAnyTag();

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task Retag() => await _retagCommandHandler.Handle(new RetagCommand());
    }
}
