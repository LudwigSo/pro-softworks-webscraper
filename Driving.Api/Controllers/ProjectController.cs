using Domain.CommandHandlers;
using Domain.Model;
using Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProjectController(
        IProjectQueriesPort projectQueries, 
        ReTagCommandHandler reTagCommandHandler
    ) : ControllerBase
    {
        private readonly IProjectQueriesPort _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
        private readonly ReTagCommandHandler _reTagCommandHandler = reTagCommandHandler ?? throw new ArgumentNullException(nameof(reTagCommandHandler));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Project>> AllActive() 
            => await _projectQueries.GetActive();

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task Retag()
            => await _reTagCommandHandler.Handle(new ReTagCommand());

    }
}
