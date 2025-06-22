using Project.DTOs.Client;

namespace Project.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project.Exceptions;
using Project.Services;


[ApiController]
[Route("api/revenue")]
public class RevenueController : ControllerBase
{
    private readonly IRevenueService _revenueService;

    public RevenueController(IRevenueService revenueService)
    {
        _revenueService = revenueService;
    }

    [HttpGet("client")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetRevenueForClient([FromBody] RevenueRequestDto dto)
    {
        try
        {
            var revenue = await _revenueService.GetRevenueForClientAsync(dto);
            return Ok(new { revenue, currency = dto.Currency });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet("product")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> GetRevenueForProduct([FromBody] RevenueRequestDto dto)
    {
        try
        {
            var revenue = await _revenueService.GetRevenueForProductAsync(dto);
            return Ok(new { revenue, currency = dto.Currency });
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (NotSupportedException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}