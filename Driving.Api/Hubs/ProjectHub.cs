using Domain;
using Application.Ports;
using Microsoft.AspNetCore.SignalR;
using Application.QueryHandlers.Dtos;

namespace Driving.Api.Hubs;

public class ProjectHub(IHubContext<ProjectHub> hubContext) : Hub, IRealtimeMessagesPort
{
    private readonly IHubContext<ProjectHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));

    public async Task NewProjectAdded(Project project)
    {
        await _hubContext.Clients.All.SendAsync("NewProjectsAdded", ProjectDto.From(project));
    }
}
