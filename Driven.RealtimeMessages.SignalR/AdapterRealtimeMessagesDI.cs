using Application.Ports;
using Domain.Ports;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Driven.RealtimeMessages.SignalR;

public static class AdapterRealtimeMessagesDi
{
    public static IHostBuilder AddAdapterRealtimeMessagesSignalR(this IHostBuilder builder)
    {
        builder = builder.ConfigureServices((context, services) =>
        {
            services.AddSignalR();
            services.AddSingleton<IRealtimeMessagesPort, ProjectHub>();
        });

        builder = builder.ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.Configure(app =>
            {
                app.UseRouting();
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapHub<ProjectHub>("/projecthub");
                });
            });
        });
        
        return builder;
    }
}

