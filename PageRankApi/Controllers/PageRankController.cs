using Microsoft.AspNetCore.Mvc;
using PageRankApi.Models;
using PageRankApi.Services;

namespace PageRankApi.Controllers;

/// <summary>
/// Controller for retrieving PageRank information.
/// </summary>
[ApiController]
[Route("[controller]")]
public class PageRankController : ControllerBase
{
    private readonly IPageRankService _pageRankService;

    /// <summary>
    /// Initializes a new instance of the <see cref="PageRankController"/> class.
    /// </summary>
    /// <param name="pageRankService">The PageRank service.</param>
    public PageRankController(IPageRankService pageRankService)
    {
        _pageRankService = pageRankService;
    }

    /// <summary>
    /// Gets the PageRank for a given host.
    /// </summary>
    /// <param name="host">The host to check. Can be a domain name or a full URL.</param>
    /// <returns>The PageRank result.</returns>
    /// <response code="200">Returns the PageRank information.</response>
    /// <response code="400">If the host is invalid or no data can be retrieved.</response>
    [HttpGet("{host}")]
    [ProducesResponseType(typeof(PageRankResult), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Get(string host)
    {
        var result = await _pageRankService.GetPageRankAsync(host);
        if (result is null)
            return BadRequest(new { error = "Unable to extract SQI" });

        return Ok(result);
    }
}