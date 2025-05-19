using PageRankApi.Models;

namespace PageRankApi.Services;

public interface IPageRankService
{
    Task<PageRankResult?> GetPageRankAsync(string host); 
}