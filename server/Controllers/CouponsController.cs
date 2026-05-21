using Microsoft.AspNetCore.Mvc;
using CouponManager.Api.DTOs;
using CouponManager.Api.Services.CouponParsing;

namespace CouponManager.Api.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
{
    private readonly ICouponParserService _parser;

    public CouponsController(ICouponParserService parser)
    {
        _parser = parser;
    }

    /// <summary>
    /// Analyzes raw coupon text and returns structured coupon data.
    /// Does not save anything to the database.
    /// </summary>
    [HttpPost("analyze")]
    [ProducesResponseType(typeof(CouponAnalysisResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult Analyze([FromBody] AnalyzeCouponRequest request)
    {
        // [ApiController] automatically returns 400 if model validation fails,
        // so if we reach here, RawText is present and at least 5 characters.
        var result = _parser.Analyze(request.RawText);
        return Ok(result);
    }
}
