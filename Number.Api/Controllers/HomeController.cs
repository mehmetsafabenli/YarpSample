using System.Net;
using Microsoft.AspNetCore.Mvc;
using Number.Abstraction.Service;

namespace Number.Api.Controllers;

[ApiController]
[Route("number/[controller]")]
public class HomeController : ControllerBase
{
    private readonly INumberService _numberService;

    public HomeController(INumberService numberService)
    {
        _numberService = numberService;
    }

    [HttpGet]
    [Route("generate-random-number")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GenerateNumber()
    {
        return Ok(await _numberService.GenerateRandomNumber());
    }

    [HttpGet("generate-random-numbers")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [ProducesResponseType((int) HttpStatusCode.BadRequest)]
    public async Task<IActionResult> GenerateRandomNumbers(int count)
    {
        return Ok(await _numberService.GenerateRandomNumbers(count));
    }
}