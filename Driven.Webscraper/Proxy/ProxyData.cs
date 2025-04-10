﻿using System.Threading;

namespace Driven.Webscraper.Proxy;

public class ProxyData(string ip, int port)
{
    public string Ip { get; } = ip;
    public int Port { get; } = port;
    public int Fails { get; private set; }
    public int Success { get; private set; }
    public int TotalSuccess => Success - Fails;

    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public string ToLog() => $"{Fails}f{Success}s; {Ip}:{Port}";

    public bool IsSameConnection(ProxyData other) => Ip == other.Ip && Port == other.Port;

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
