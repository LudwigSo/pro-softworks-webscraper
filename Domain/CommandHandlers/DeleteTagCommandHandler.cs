using Domain.Ports;
using Domain.Ports.Queries;

namespace Domain.CommandHandlers;

public record DeleteTagCommand(int Id);
public class DeleteTagCommandHandler(
    ILogger logger,
    ITagQueriesPort tagQueriesPort,
    IWriteContext writeContext
    )
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly ITagQueriesPort _tagQueriesPort = tagQueriesPort ?? throw new ArgumentNullException(nameof(tagQueriesPort));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));

    public async Task Handle(DeleteTagCommand command)
    {
        var tagToDelete = await _tagQueriesPort.GetTag(command.Id);
        if (tagToDelete is null)
        {
            _logger.LogInformation($"Tag cannot be deleted because it does not exist: {command.Id}");
            return;
        }

        _writeContext.Remove(tagToDelete);
        await _writeContext.SaveChangesAsync();
        _logger.LogInformation($"Deleted tag {tagToDelete.Id}:{tagToDelete.Name}");
    }
}
