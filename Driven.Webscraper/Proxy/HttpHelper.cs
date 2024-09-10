using Domain.Ports;
using HtmlAgilityPack;

namespace Driven.Webscraper.Proxy;

public class HttpHelper(ILogger logger, HttpClientFactory httpClientFactory)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    public async Task<string?> Get(
        string url,
        (HttpClient httpClient, ProxyData proxyData)? httpClientAndProxy = null,
        int tryNumber = 1)
    {
        httpClientAndProxy ??= await _httpClientFactory.CreateWithProxy();
        var httpClient = httpClientAndProxy.Value.httpClient;
        var proxyData = httpClientAndProxy.Value.proxyData;

        try
        {
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception e)
        {
            await proxyData.IncrementFails();
            _logger.LogInformation($"Failed getting page ({proxyData.Fails}) with {proxyData.Ip}:{proxyData.Port} from {url}");
            if (tryNumber < 20)
            {
                return await Get(url, null, tryNumber + 1);
            }
            _logger.LogError(e.Message);
        }
        return null;

    }

    public async Task<HtmlDocument?> GetHtml(
        string url,
        (HttpClient httpClient, ProxyData proxyData)? httpClientAndProxy = null,
        int tryNumber = 1)
    {
        httpClientAndProxy ??= await _httpClientFactory.CreateWithProxy();
        var html = await Get(url, httpClientAndProxy);
        if (html == null) return null;
        var htmlDocument = new HtmlDocument();

        try
        {
            htmlDocument.LoadHtml(html);
            if (htmlDocument.ParseErrors.Any()) throw new InvalidDataException("Parse errors during LoadHtml exist.");
            if (htmlDocument.DocumentNode.SelectSingleNode("//body") == null) throw new InvalidDataException("No body tag found in document.");
            if (htmlDocument.ParsedText.Contains("Internal Server Error") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Internal Server Error in document.");
            if (htmlDocument.ParsedText.Contains("502 Bad Gateway") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Bad Gateway Error in document.");
        }
        catch (Exception e)
        {
            await httpClientAndProxy.Value.proxyData.IncrementFails();
            _logger.LogInformation($"Failed getting html ({httpClientAndProxy.Value.proxyData.Fails}) with {httpClientAndProxy.Value.proxyData.Ip}:{httpClientAndProxy.Value.proxyData.Port} from {url}");
            if (tryNumber < 20)
            {
                return await GetHtml(url, null, tryNumber + 1);
            }
            _logger.LogError(e.Message);
        }

        return htmlDocument;
    }
}
