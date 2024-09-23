﻿using Domain;
using Application.Ports;
using Microsoft.AspNetCore.SignalR;
using Application.QueryHandlers.Dtos;

namespace Driven.RealtimeMessages.SignalR;

public class ProjectHub(IHubContext<ProjectHub> hubContext) : Hub, IRealtimeMessagesPort
{
    private readonly IHubContext<ProjectHub> _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    public async Task NewProjectsAdded(IEnumerable<Project> projects)
    {
        await _hubContext.Clients.All.SendAsync("NewProjectsAdded", projects.Select(ProjectDto.From));
    }
}
    