using Domain.Ports;
using HtmlAgilityPack;
using PlainHttp;
using System;

namespace Driven.Webscraper.Proxy;

public class HttpHelper(ILogger logger, IProxyLoader proxyLoader)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IProxyLoader _proxyLoader = proxyLoader ?? throw new ArgumentNullException(nameof(proxyLoader));
    private ProxyData[] _proxies = [];
    private readonly Random _random = new();
    private readonly SemaphoreSlim _getHtmlSemaphore = new(1, 1);

    public async Task<HtmlDocument> GetHtml(string url, bool withProxy = true, int retry = 0, int retryDelayInMs = 0)
    {
        if (withProxy && _proxies.Length == 0)
        {
            await _getHtmlSemaphore.WaitAsync();
            try
            {
                await InitializeProxies();
                if (_proxies.Length == 0) throw new InvalidOperationException("no proxies available");
            }
            finally
            {
                _getHtmlSemaphore.Release();
            }
        }

        ProxyData? proxyData = null;
        HtmlDocument htmlDocument;
        try
        {
            proxyData = withProxy ? NextRandomProxyData() : null;
            htmlDocument = await GetHtml(url, proxyData);
            if (proxyData != null) await proxyData.IncrementSuccess();
            _logger.LogDebug($"Succeeded getting html page retry: {retry}, {proxyData?.ToLog() ?? "no proxy"}, from {url}");
        }
        catch (Exception e)
        {
            if (proxyData != null) await proxyData.IncrementFails();

            _logger.LogDebug($"Failed getting html page retry: {retry}, {proxyData?.ToLog() ?? "no proxy"}, from {url}");
            if (retry < 10)
            {
                if (retryDelayInMs > 0) await Task.Delay(retryDelayInMs);
                return await GetHtml(url, withProxy, retry + 1);
            }
            _logger.LogError($"Fatal Failed getting html page retry: {retry}, {proxyData?.ToLog() ?? "no proxy"}, from {url} with message {e.Message}");
            throw;
        }

        return htmlDocument;
    }

    public async Task InitializeProxies()
    {
        _logger.LogInformation("Start to initialize proxies");
        _proxies = await _proxyLoader.LoadAvailableProxies();
        if (_proxies.Length == 0) throw new InvalidOperationException("no proxies available");
        
        _logger.LogInformation($"{_proxies.Length} proxies were found");

        var proxyTasks = new List<Task>();
        foreach (var proxy in _proxies)
        {
            var task = TestProxy(proxy);
            proxyTasks.Add(task);
        }
        Task.WaitAll([.. proxyTasks]);

        _proxies = _proxies.Where(p => p.Fails == 0).ToArray();
        _logger.LogInformation($"{_proxies.Length} proxies responded");
    }

    internal async Task TestProxy(ProxyData proxyData)
    {
        try
        {
            await GetHtml("http://ping.eu/proxy/", proxyData);
            await proxyData.IncrementSuccess();
            _logger.LogDebug($"Succeeded proxytest: {proxyData.ToLog()}");
        }
        catch
        {
            await proxyData.IncrementFails();
            _logger.LogDebug($"Failed proxytest: {proxyData.ToLog()}");
        }
    }

    internal async Task<HtmlDocument> GetHtml(string url, ProxyData? proxyData)
    {
        var htmlDocument = new HtmlDocument();
        var request = NewRequest(url, proxyData);
        var response = await request.SendAsync();
        if (response.Succeeded == false) throw new InvalidOperationException($"Request failed with status code {response.StatusCode}.");
        var responseString = await response.ReadString();

        htmlDocument.LoadHtml(responseString.Trim());
        //if (htmlDocument.ParseErrors.Any()) throw new InvalidDataException("Parse errors during LoadHtml exist.");
        if (htmlDocument.DocumentNode.SelectSingleNode("//body") == null) throw new InvalidDataException("No body tag found in document.");
        if (htmlDocument.ParsedText.Contains("Internal Server Error") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Internal Server Error in document.");
        if (htmlDocument.ParsedText.Contains("502 Bad Gateway") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Bad Gateway Error in document.");

        return htmlDocument;
    }

    internal HttpRequest NewRequest(string url, ProxyData? proxyData)
    {
        var request = new HttpRequest(url);
        request.Method = HttpMethod.Get;
        request.Timeout = TimeSpan.FromSeconds(10);
        request.Headers = new Dictionary<string, string>()
        {
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36" }
        };
        if (proxyData != null)
        {
            var proxyUriBuilder = new UriBuilder(proxyData.Ip);
            proxyUriBuilder.Port = proxyData.Port;
            request.Proxy = proxyUriBuilder.Uri;
        }
        return request;
    }

    internal ProxyData NextRandomProxyData()
    {
        if (_proxies.Length == 0) throw new InvalidOperationException($"{_proxies.Length} proxies in memory");
        var averageProxySuccess = _proxies.OrderByDescending(p => p.TotalSuccess).ToArray()[Math.Max(0, (_proxies.Length/2)-1)].TotalSuccess;
       var availableProxies = _proxies.Where(p => p.TotalSuccess >= averageProxySuccess).ToArray();
        return availableProxies[_random.Next(availableProxies.Length)];
    }
}
