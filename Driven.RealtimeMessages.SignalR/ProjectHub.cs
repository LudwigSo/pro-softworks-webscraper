using Domain.Model;
using Domain.Ports;
using Microsoft.AspNetCore.SignalR;

namespace Driven.RealtimeMessages.SignalR;

public class ProjectHub(IHubContext<ProjectHub> hubContext) : Hub, IRealtimeMessagesPort
{
    private readonly IHubContext<ProjectHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    public async Task NewProjectsAdded(List<Project> projects)
    {
        await _hubContext.Clients.All.SendAsync("NewProjectsAdded", projects);
    }

    public async Task ProjectsRemoved(List<Project> projects)
    {
        await _hubContext.Clients.All.SendAsync("ProjectsRemoved", projects);
    }
}
    