using System.Threading;

namespace Driven.Webscraper.Proxy;

public class ProxyData(string ip, int port, double averageTimeout, int fails = 0)
{
    public string Ip { get; } = ip;
    public int Port { get; } = port;
    public double AverageTimeout { get; } = averageTimeout;
    public int Fails { get; private set; } = fails;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task IncrementFails() {
        await _semaphore.WaitAsync();
        try
        {
            Fails = Fails + 1;
        }
        finally
        {
            _semaphore.Release();
        }
    }
};
