using Domain;
using Application.CommandHandlers;
using Application.QueryHandlers;
using Microsoft.AspNetCore.Mvc;
using Application.QueryHandlers.Dtos;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Driving.Api.Controllers;

[ApiController]
[Route("[controller]/[action]/")]
public class ProjectController(
    ClaimProjectCommandHandler claimProjectCommandHandler,
    ProjectQueryHandler projectQueryHandler, 
    TagCommandHandler tagCommandHandler
) : ControllerBase
{
    private readonly ClaimProjectCommandHandler _claimProjectCommandHandler = claimProjectCommandHandler ?? throw new ArgumentNullException(nameof(claimProjectCommandHandler));
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
    public async Task Retag() => await _tagCommandHandler.Handle(new RetagCommand());

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Claim(int projectId)
    {
        var username = User.Claims.First(c => c.Type == "preferred_username").Value;
        await _claimProjectCommandHandler.Handle(new ClaimProjectCommand(projectId, username));
    }
}
