using System.Threading;

namespace Driven.Webscraper.Proxy;

public class ProxyData(string ip, int port)
{
    public string Ip { get; } = ip;
    public int Port { get; } = port;
    public int Fails { get; private set; }
    public int Success { get; private set; }

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public string ToLog() => $"{Fails}f{Success}s; {Ip}:{Port}";

    public bool IsAvailable()
    {
        if (Success == 0) return Fails < 2;
        return Fails / Success < 0.2;
    }

    public async Task IncrementFails() {
        await _semaphore.WaitAsync();
        Fails = Fails + 1;
        _semaphore.Release();
    }

    public async Task IncrementSuccess()
    {
        await _semaphore.WaitAsync();
        Success = Success + 1;
        _semaphore.Release();
    }
};
