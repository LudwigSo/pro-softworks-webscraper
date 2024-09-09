using System.Text.Json;

namespace Driven.Webscraper.Proxy;

public class ProxyscrapeLoader : IProxyLoader
{
    private const string URL = "https://api.proxyscrape.com/v3/free-proxy-list/get?request=displayproxies&protocol=http&proxy_format=ipport&format=json&timeout=1000";
    private const string FILE_PATH = "ProxyscrapeLoader.json";
    private SemaphoreSlim _loadAvailableProxiesSemaphore = new(1, 1);
    public async Task<ProxyData[]> LoadAvailableProxies()
    {
        await _loadAvailableProxiesSemaphore.WaitAsync();
        JsonDocument? json = null;
        try
        { 
            if (!File.Exists(FILE_PATH) || FileWasWrittenMoreThan5MinutesAgo())
            {
                using HttpClient client = new();
                var jsonString = await client.GetStringAsync(URL);
                await File.WriteAllTextAsync(FILE_PATH, jsonString);
            }

            var jsonStringFromFile = await File.ReadAllTextAsync(FILE_PATH);
            json = JsonDocument.Parse(jsonStringFromFile);
        }
        finally
        {
            _loadAvailableProxiesSemaphore.Release();
        }
        return ParseJsonProxyData(json).ToArray();
    }

    private bool FileWasWrittenMoreThan5MinutesAgo()
    {
        var lastWrite = File.GetLastWriteTimeUtc(FILE_PATH);
        return DateTime.UtcNow - lastWrite > TimeSpan.FromMinutes(5);
    }

    private IEnumerable<ProxyData> ParseJsonProxyData(JsonDocument json)
    {
        var root = json.RootElement;
        var proxyList = root.GetProperty("proxies");
        if (proxyList.ValueKind != JsonValueKind.Array)
        {
            throw new InvalidOperationException("Invalid JSON format");
        }

        foreach (var proxyData in proxyList.EnumerateArray())
        {
            var ip = proxyData.GetProperty("ip").GetString();
            var port = proxyData.GetProperty("port").GetInt32();
            if (ip == null || port == 0) continue;

            var protocol = proxyData.GetProperty("protocol").GetString();
            if (protocol != "http") continue;

            var averageTimeout = proxyData.GetProperty("average_timeout").GetDouble();
            if (averageTimeout > 1000) continue;

            var alive = proxyData.GetProperty("alive").GetBoolean();
            if (!alive) continue;

            yield return new ProxyData(ip, port, averageTimeout);
        }
    }
}
