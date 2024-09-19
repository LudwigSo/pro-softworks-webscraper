using Domain;

namespace Application.Ports;

public interface IRealtimeMessagesPort
{
    Task NewProjectsAdded(IEnumerable<Project> projects);
}