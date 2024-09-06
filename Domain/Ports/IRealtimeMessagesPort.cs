using Domain.Model;

namespace Domain.Ports;

public interface IRealtimeMessagesPort
{
    Task NewProjectsAdded(IEnumerable<Project> projects);
    Task ProjectsRemoved(IEnumerable<Project> projects);
}