using System.Net;
using System.Net.Http;

namespace Driven.Webscraper.Proxy;

public class HttpClientFactory(IProxyLoader proxyLoader)
{
    private readonly IProxyLoader _proxyLoader = proxyLoader ?? throw new ArgumentNullException(nameof(proxyLoader));
    private ProxyData[] _proxies = [];
    private readonly Random _random = new();

    internal async Task<(HttpClient httpClient, ProxyData proxyData)> CreateWithProxy()
    {
        if (_proxies.Length == 0) _proxies = await _proxyLoader.LoadAvailableProxies();
        if (_proxies.Length == 0) throw new InvalidOperationException("no proxies available");

        var proxyData = NextRandomProxyData(failThreshold: 1);

        var httpClientHandler = new HttpClientHandler
        {
            Proxy = new WebProxy(proxyData.Ip, proxyData.Port),
            UseProxy = true,
        };

        return (new HttpClient(httpClientHandler), proxyData);
    }

    internal ProxyData NextRandomProxyData(int failThreshold)
    {
        var availableProxies = _proxies.Where(p => p.Fails < failThreshold).ToArray();
        if (availableProxies.Length == 0) throw new InvalidOperationException($"no proxies with less than {failThreshold} available");
        return availableProxies[_random.Next(availableProxies.Length)];
    }
}
