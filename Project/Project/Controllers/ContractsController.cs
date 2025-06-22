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

    [HttpPost("payment")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> AddPayment([FromBody] AddPaymentDto dto)
    {
        try
        {
            var remaining = await _contractService.AddPaymentAsync(dto);

            if (remaining == 0)
                return Ok("Contract fully paid and signed.");
            else
                return Ok("Payment accepted. Remaining amount: " + remaining + " PLN.");
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

    [HttpDelete("{contractId}")]
    [Authorize(Roles = "Admin,User")]
    public async Task<IActionResult> DeleteContract(int contractId)
    {
        try
        {
            await _contractService.DeleteContractAsync(contractId);
            return NoContent();
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal server error" });
        }
    }

    
}