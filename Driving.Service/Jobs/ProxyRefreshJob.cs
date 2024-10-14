using Driven.Webscraper.Proxy;
using Quartz;

namespace Driving.Service.Jobs;

[DisallowConcurrentExecution]
internal class ProxyRefreshJob(ILogger<ProxyRefreshJob> logger, HttpHelper httpHelper) : IJob
{
    private ILogger Logger { get; } = logger ?? throw new ArgumentNullException(nameof(logger));
    private HttpHelper HttpHelper { get; } = httpHelper ?? throw new ArgumentNullException(nameof(httpHelper));

    public async Task Execute(IJobExecutionContext context)
    {
        Logger.LogInformation("Start to refresh Proxies");
        var newProxies = HttpHelper.Proxies.ToList();
        if (newProxies.Count > 15)
        {
            var amountToRemove = newProxies.Count / 5;
            var proxiesToRemove = newProxies.OrderBy(p => p.TotalSuccess).Take(amountToRemove);
            foreach (var proxyToRemove in proxiesToRemove)
            {
                newProxies.Remove(proxyToRemove);
            }
            Logger.LogInformation($"Removed the worst performing {amountToRemove} proxies");
        }

        var availableProxiesFromApi = await HttpHelper.InitializeProxies();
        var proxiesToAdd = availableProxiesFromApi.Where(np => !HttpHelper.Proxies.Any(op => np.IsSameConnection(op))).ToList();

        newProxies.AddRange(proxiesToAdd);
        Logger.LogInformation($"Added {proxiesToAdd} proxies");

        HttpHelper.Proxies = [.. newProxies];
        Logger.LogInformation($"Now {HttpHelper.Proxies.Length} proxies are available");
    }
}
