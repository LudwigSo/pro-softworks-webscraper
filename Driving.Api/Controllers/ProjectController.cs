using Domain;
using Application.CommandHandlers;
using Application.QueryHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]/")]
    public class ProjectController(
        ProjectQueryHandler projectQueryHandler, 
        TagCommandHandler tagCommandHandler
    ) : ControllerBase
    {
        private readonly ProjectQueryHandler _projectQueryHandler = projectQueryHandler ?? throw new ArgumentNullException(nameof(projectQueryHandler));
        private readonly TagCommandHandler _tagCommandHandler = tagCommandHandler ?? throw new ArgumentNullException(nameof(tagCommandHandler));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Project>> AllActiveWithAnyTag() => await _projectQueryHandler.GetActiveWithAnyTag();

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task Retag() => await tagCommandHandler.Handle(new RetagCommand());
    }
}
