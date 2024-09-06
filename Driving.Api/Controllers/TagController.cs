using Domain.CommandHandlers;
using Domain.Model;
using Domain.Ports.Queries;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]/")]
    public class TagController(
        ITagQueriesPort tagQueries, 
        CreateTagCommandHandler createTagCommandHandler,
        DeleteTagCommandHandler deleteTagCommandHandler
    ) : ControllerBase
    {
        private readonly ITagQueriesPort _tagQueries = tagQueries ?? throw new ArgumentNullException(nameof(tagQueries));
        private readonly CreateTagCommandHandler _createTagCommandHandler = createTagCommandHandler ?? throw new ArgumentNullException(nameof(createTagCommandHandler));
        private readonly DeleteTagCommandHandler _deleteTagCommandHandler = deleteTagCommandHandler ?? throw new ArgumentNullException(nameof(deleteTagCommandHandler));

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<Tag>> All() => await _tagQueries.GetAllTags();

        [Route("/{id}")]
        [HttpDelete]
        [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
        public async Task Delete(int id) => await _deleteTagCommandHandler.Handle(new DeleteTagCommand(id));

        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task Create(CreateTagCommand command) => await _createTagCommandHandler.Handle(command);
    }
}
