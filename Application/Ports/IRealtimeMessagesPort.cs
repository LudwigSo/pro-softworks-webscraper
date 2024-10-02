using Domain;

namespace Application.Ports;

public interface IRealtimeMessagesPort
{
    Task NewProjectAdded(Project project);
}