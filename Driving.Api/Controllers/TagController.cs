using Application.CommandHandlers;
using Application.QueryHandlers;
using Application.QueryHandlers.Dtos;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers;

[ApiController]
[Route("[controller]/[action]/")]
public class TagController(
    TagQueryHandler tagQueryHandler, 
    TagCommandHandler tagCommandHandler
) : ControllerBase
{
    private readonly TagQueryHandler _tagQueryHandler = tagQueryHandler ?? throw new ArgumentNullException(nameof(tagQueryHandler));
    private readonly TagCommandHandler _tagCommandHandler = tagCommandHandler ?? throw new ArgumentNullException(nameof(tagCommandHandler));

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TagDto>), StatusCodes.Status200OK)]
    public async Task<IEnumerable<TagDto>> All() => await _tagQueryHandler.Handle(new AllTagsQuery());

    [Route("/{id}")]
    [HttpDelete]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Delete(int id) => await _tagCommandHandler.Handle(new DeleteTagCommand(id));

    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Create(CreateTagCommand command) => await _tagCommandHandler.Handle(command);
}
