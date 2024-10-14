using Application.Ports;
using Domain;
using Driven.Persistence.Postgres;
using Microsoft.Extensions.Logging;

namespace Application.CommandHandlers;

public record CreateKeywordCommand(int TagId, string Value);
public record DeleteKeywordCommand(int Id);

public class KeywordCommandHandler(ILogger<KeywordCommandHandler> logger, Context dbContext)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly Context _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));

    public async Task Handle(CreateKeywordCommand command)
    {
        var keyword = new Keyword(command.TagId, command.Value);
        await _dbContext.Keywords.AddAsync(keyword);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Added keyword {keyword.Id}:{keyword.Value}");
    }

    public async Task Handle(DeleteKeywordCommand command)
    {
        var keywordToDelete = await _dbContext.Keywords.FindAsync(command.Id);
        if (keywordToDelete is null)
        {
            _logger.LogInformation($"Keyword cannot be deleted because it does not exist: {command.Id}");
            return;
        }

        _dbContext.Keywords.Remove(keywordToDelete);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Deleted keyword {keywordToDelete.Id}:{keywordToDelete.Value}");
    }
}
