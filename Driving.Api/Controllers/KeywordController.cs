using Application.CommandHandlers;
using Application.QueryHandlers;
using Application.QueryHandlers.Dtos;
using Domain;
using Microsoft.AspNetCore.Mvc;

namespace Driving.Api.Controllers;

[ApiController]
[Route("[controller]/[action]/")]
public class KeywordController(KeywordCommandHandler keywordCommandHandler) : ControllerBase
{
    private readonly KeywordCommandHandler _keywordCommandHandler = keywordCommandHandler ?? throw new ArgumentNullException(nameof(keywordCommandHandler));

    [Route("/{id}")]
    [HttpDelete]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Delete(int id) => await _keywordCommandHandler.Handle(new DeleteKeywordCommand(id));

    [HttpPost]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    public async Task Create(CreateKeywordCommand command) => await _keywordCommandHandler.Handle(command);
}
