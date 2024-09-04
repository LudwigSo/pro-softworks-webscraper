using Application.Ports;
using Domain.Model;
using Domain.Ports;
using Microsoft.AspNetCore.SignalR;

namespace Driven.RealtimeMessages.SignalR;

public class ProjectHub : Hub, IRealtimeMessagesPort
{
    public async Task NewProjectsAdded(List<Project> projects)
    {
        await Clients.All.SendAsync("NewProjectsAdded", projects);
    }

    public async Task ProjectsRemoved(List<Project> projects)
    {
        await Clients.All.SendAsync("ProjectsRemoved", projects);
    }
}
    