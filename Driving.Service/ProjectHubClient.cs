using Application.Ports;
using Domain;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Driving.Service;

public static class SignalRDi
{
    public static IServiceCollection AddRealtimeMessagesSignalR(this IServiceCollection services)
    {
        services.AddSingleton<IRealtimeMessagesPort, ProjectHubClient>();
        return services;
    }
}

public class ProjectHubClient : IRealtimeMessagesPort
{
    private readonly HubConnection _connection;

    public ProjectHubClient()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("http://drivingapi:8080/projectHub")
            .Build();
    }

    public async Task NewProjectAdded(Project project)
    {
        if (_connection.State == HubConnectionState.Disconnected)
        {
            await _connection.StartAsync();
        }
        await _connection.InvokeAsync("NewProjectAdded", project);
    }

    public async Task StopAsync()
    {
        await _connection.StopAsync();
    }
}