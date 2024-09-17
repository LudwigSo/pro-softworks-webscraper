using Domain.Ports;
using HtmlAgilityPack;

namespace Driven.Webscraper.Proxy;

public record ProxyResponse(HtmlDocument HtmlDocument, ProxyData UsedProxy);

public class HttpHelper(ILogger logger, HttpClientFactory httpClientFactory)
{
    private readonly ILogger _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly HttpClientFactory _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

    public async Task<HtmlDocument> GetHtml(
        string url,
        (HttpClient HttpClient, ProxyData ProxyData)? httpClientAndProxy = null,
        int tryNumber = 1)
    {
        httpClientAndProxy ??= await _httpClientFactory.CreateWithProxy();
        var httpClient = httpClientAndProxy.Value.HttpClient;
        var proxyData = httpClientAndProxy.Value.ProxyData;

        var htmlDocument = new HtmlDocument();
        try
        {
            var response = await httpClientAndProxy.Value.HttpClient.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            htmlDocument.LoadHtml(responseString);
            if (htmlDocument.ParseErrors.Any()) throw new InvalidDataException("Parse errors during LoadHtml exist.");
            if (htmlDocument.DocumentNode.SelectSingleNode("//body") == null) throw new InvalidDataException("No body tag found in document.");
            if (htmlDocument.ParsedText.Contains("Internal Server Error") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Internal Server Error in document.");
            if (htmlDocument.ParsedText.Contains("502 Bad Gateway") && htmlDocument.ParsedText.Length < 250) throw new InvalidDataException("Bad Gateway Error in document.");
            await proxyData.IncrementSuccess();
            _logger.LogDebug($"Succeeded getting html page try: {tryNumber}, {proxyData.ToLog()}, from {url}");
        }
        catch (Exception e)
        {
            await proxyData.IncrementFails();
            _logger.LogDebug($"Failed getting html page try: {tryNumber}, {proxyData.ToLog()}, from {url}");
            if (tryNumber < 10)
            {
                return await GetHtml(url, null, tryNumber + 1);
            }
            _logger.LogError(e.Message);
            throw;
        }

        return htmlDocument;
    }
}
