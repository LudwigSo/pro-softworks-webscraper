namespace Driven.Webscraper.Proxy;

public class ProxyData(string ip, int port, double averageTimeout, int fails = 0)
{
    public string Ip { get; } = ip;
    public int Port { get; } = port;
    public double AverageTimeout { get; } = averageTimeout;
    public int Fails { get; private set; } = fails;

    public void IncrementFails() => Fails = Fails++;
};
