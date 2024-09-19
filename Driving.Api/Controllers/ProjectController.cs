using Domain;
using Application.CommandHandlers;
using Application.QueryHandlers;
using Microsoft.AspNetCore.Mvc;
using Application.QueryHandlers.Dtos;

namespace Driving.Api.Controllers;

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
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<ProjectDto>> AllWithAnyTag(DateTime? since = null)
    {
        var query = new ProjectsWithAnyTagQuery(since ?? TimeProvider.System.GetLocalNow().DateTime.AddDays(-7));
        return await _projectQueryHandler.Handle(query);
    }

    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Retag() => await tagCommandHandler.Handle(new RetagCommand());
}
