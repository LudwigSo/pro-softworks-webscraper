using Domain.Model;
using Domain.Ports;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectQueriesPort _projectQueries;

        public ProjectController(IProjectQueriesPort projectQueries)
        {
            _projectQueries = projectQueries ?? throw new ArgumentNullException(nameof(projectQueries));
        }


        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Project>> AllActive() 
            => await _projectQueries.GetActive(); 
    }
}
