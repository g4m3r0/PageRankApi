using Microsoft.AspNetCore.Mvc;
using PageRankApi.Services;

namespace PageRankApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PageRankController : ControllerBase
{
    private readonly IPageRankService _pageRankService;

    public PageRankController(IPageRankService pageRankService)
    {
        _pageRankService = pageRankService;
    }

    [HttpGet("{host}")]
    public async Task<IActionResult> Get(string host)
    {
        var result = await _pageRankService.GetPageRankAsync(host);
        if (result is null)
            return BadRequest(new { error = "Unable to extract SQI" });

        return Ok(result);
    }
}