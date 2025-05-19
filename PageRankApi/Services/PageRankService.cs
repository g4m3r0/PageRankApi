using PageRankApi.Models;

namespace PageRankApi.Services;

using System.Text.Json;
using System.Text.RegularExpressions;

public class PageRankService : IPageRankService
{
    private readonly HttpClient _httpClient;
    private readonly IDictionary<int, int> _thresholds;

    private static readonly Regex SqiRegex = new Regex(
        @"""sqi""\s*:\s*(\d+).*?""type""\s*:\s*""SQI""",
        RegexOptions.Compiled | RegexOptions.Singleline
    );

    public PageRankService(IHttpClientFactory httpFactory, IConfiguration config)
    {
        _httpClient = httpFactory.CreateClient();
        // Load thresholds as Dictionary<PR, MinSqi>
        _thresholds = config
            .GetSection("PageRankThresholds")
            .Get<Dictionary<string, int>>()
            .ToDictionary(k => int.Parse(k.Key), v => v.Value);
    }

    public async Task<PageRankResult?> GetPageRankAsync(string host)
    {
        var html = await _httpClient
            .GetStringAsync($"https://webmaster.yandex.ru/siteinfo/?host={host}");

        var sqi = ParseSqi(html);
        if (sqi < 0) return null;

        var pr = MapToPageRank(sqi);
        return new PageRankResult(host, sqi, pr);
    }

    private int ParseSqi(string html)
    {
        var m = SqiRegex.Match(html);
        if (!m.Success)
            return -1;

        // group[1] is the digits after "sqi":
        return int.Parse(m.Groups[1].Value);
    }

    private int MapToPageRank(int sqi)
    {
        // Highest PR whose threshold ≤ sqi
        return _thresholds
            .OrderBy(kvp => kvp.Value)
            .LastOrDefault(kvp => sqi >= kvp.Value)
            .Key;
    }
}