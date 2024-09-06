using Domain.Model;
using Domain.Ports;

namespace Domain.CommandHandlers;

public record CreateTagCommand(string Name);
public class CreateTagCommandHandler(
    ILogger logger,
    IWriteContext writeContext
    )
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IWriteContext _writeContext = writeContext ?? throw new ArgumentNullException(nameof(writeContext));

    public async Task Handle(CreateTagCommand command)
    {
        var tag = new Tag(command.Name);
        await _writeContext.AddRange([tag]);
        await _writeContext.SaveChangesAsync();
        _logger.LogInformation($"Added tag {tag.Id}:{tag.Name}");
    }
}
