using Domain.Model;

namespace Domain.Ports;

public interface IRealtimeMessagesPort
{
    Task NewProjectsAdded(List<Project> projects);
    Task ProjectsRemoved(List<Project> projects);
}