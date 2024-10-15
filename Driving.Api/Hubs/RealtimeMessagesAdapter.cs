using Application.Ports;

namespace Driving.Api.Hubs;

public static class RealtimeMessagesAdapter
{
    public static IServiceCollection AddRealtimeMessagesAdapter(this IServiceCollection services)
    {
        services.AddSingleton<IRealtimeMessagesPort, ProjectHub>();
        return services;
    }
}
