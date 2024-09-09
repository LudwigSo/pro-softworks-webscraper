namespace Driven.Webscraper.Proxy;

public interface IProxyLoader
{
    Task<ProxyData[]> LoadAvailableProxies();
}
