using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Project.DTOs.Contract;
using Project.Exceptions;
using Project.Services;

namespace Project.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpPost("upfront")]
    [Authorize(Roles = "Admin,User")]
public async Task<IActionResult> CreateContract([FromBody] CreateContractDto dto)
    {
        try
        {
           var result = await _contractService.CreateUpfrontContractAsync(dto);
            return Created("","Contract created. The final price is: " + result + " PLN.");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (BadHttpRequestException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal server error" });
        }
    }

    [HttpPatch("sign")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> SignContract([FromBody] SignContractDto dto)
    {
        try
        {
            await _contractService.SignContractAsync(dto);
            return Ok("Contract signed successfully.");
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}