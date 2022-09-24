using System.Net;
using Color.Abstraction.Service;
using Microsoft.AspNetCore.Mvc;

namespace Color.Api.Controllers;

[ApiController]
[Route("color/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IColorService _colorService;

    public HomeController(IColorService colorService)
    {
        _colorService = colorService;
    }

    [HttpGet]
    [Route("get-random-color")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GenerateColor()
    {
        return Ok(await _colorService.GenerateRandomColor());
    }

    [HttpGet]
    [Route("get-random-colors")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    public async Task<ActionResult> GenerateColors(int count)
    {
        return Ok(await _colorService.GenerateRandomColors(count));
    }
}