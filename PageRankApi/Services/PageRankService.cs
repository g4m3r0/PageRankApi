using PageRankApi.Models;
using System;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PageRankApi.Services;

public class PageRankService : IPageRankService
{
    private readonly HttpClient _httpClient;
    private readonly IDictionary<int, int> _thresholds;

    private static readonly Regex SqiRegex = new Regex(
        @"""sqi""\s*:\s*(\d+).*?""type""\s*:\s*""SQI""",
        RegexOptions.Compiled | RegexOptions.Singleline
    );

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRankService"/> class.
    /// </summary>
    /// <param name="httpFactory">The HTTP client factory.</param>
    /// <param name="config">The configuration.</param>
    public PageRankService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpClient = httpFactory.CreateClient();
        // Load thresholds as Dictionary<PR, MinSqi>
        _thresholds = config
            .GetSection("PageRankThresholds")
            .Get<Dictionary<string, int>>()
            .ToDictionary(k => int.Parse(k.Key), v => v.Value);
    }

    /// <summary>
    /// Gets the PageRank for a given host.
    /// </summary>
    /// <param name="host">The host to check.</param>
    /// <returns>A <see cref="PageRankResult"/> or null if the host is invalid or has no data.</returns>
    public async Task<PageRankResult?> GetPageRankAsync(string host)
    {
        var cleanedHost = CleanHost(host);
        if (string.IsNullOrEmpty(cleanedHost))
        {
            return null;
        }

        var html = await _httpClient
            .GetStringAsync($"https://webmaster.yandex.ru/siteinfo/?host={cleanedHost}");

        var sqi = ParseSqi(html);
        if (sqi < 0) return null;

        var pr = MapToPageRank(sqi);
        return new PageRankResult(cleanedHost, sqi, pr);
    }

    /// <summary>
    /// Cleans the input string to extract a valid hostname.
    /// </summary>
    /// <param name="host">The user-provided host string.</param>
    /// <returns>
    /// A URL-encoded hostname if parsing succeeds; otherwise, the original host string.
    /// </returns>
    private string CleanHost(string host)
    {
        if (string.IsNullOrWhiteSpace(host))
        {
            return string.Empty;
        }

        var decodedHost = WebUtility.UrlDecode(host);

        var potentialUrl = decodedHost;
        if (!potentialUrl.Contains("://"))
        {
            potentialUrl = "http://" + potentialUrl;
        }

        if (Uri.TryCreate(potentialUrl, UriKind.Absolute, out var uri))
        {
            return WebUtility.UrlEncode(uri.Host);
        }

        return host;
    }

    /// <summary>
    /// Parses the Site Quality Index (SQI) from the Yandex Webmaster HTML response.
    /// </summary>
    /// <param name="html">The HTML content.</param>
    /// <returns>The SQI value, or -1 if not found.</returns>
    private int ParseSqi(string html)
    {
        var m = SqiRegex.Match(html);
        if (!m.Success)
            return -1;

        // group[1] is the digits after "sqi":
        return int.Parse(m.Groups[1].Value);
    }

    /// <summary>
    /// Maps the Site Quality Index (SQI) to a PageRank score based on configured thresholds.
    /// </summary>
    /// <param name="sqi">The SQI value.</param>
    /// <returns>The corresponding PageRank score.</returns>
    private int MapToPageRank(int sqi)
    {
        // Highest PR whose threshold ≤ sqi
        return _thresholds
            .OrderBy(kvp => kvp.Value)
            .LastOrDefault(kvp => sqi >= kvp.Value)
            .Key;
    }
}