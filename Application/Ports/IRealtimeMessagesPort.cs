using Domain.Model;

namespace Application.Ports;

public interface IRealtimeMessagesPort
{
    Task NewProjectsAdded(List<Project> projects);
    Task ProjectsRemoved(List<Project> projects);
}