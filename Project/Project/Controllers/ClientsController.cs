using Project.DTOs.Client;
using Project.Exceptions;
using Project.Services;

namespace Project.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
    }

    [HttpPost("individual")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddIndividualClient([FromBody] AddIndividualClientDto dto)
    {
        try
        {
            await _clientService.AddIndividualClientAsync(dto);
            return Created();
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal server error" });
        }
    }

    [HttpPost("company")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddCompanyClient([FromBody] AddCompanyClientDto dto)
    {
        try
        {
            await _clientService.AddCompanyClientAsync(dto);
            return Created();
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal server error" });
        }
    }
    
    [HttpDelete("individual/{pesel}")] 
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteClient(string pesel)
    {
        try
        {
            await _clientService.RemoveClientByPeselAsync(pesel);
            return NoContent();
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return StatusCode(StatusCodes.Status500InternalServerError,
                new { error = "Internal server error" });
        }
    }
    
    [HttpPatch("individual")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateIndividualClient([FromBody] UpdateIndividualClientDto dto)
    {
        try
        {
            await _clientService.UpdateIndividualClientAsync(dto);
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

    [HttpPatch("company")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCompanyClient([FromBody] UpdateCompanyClientDto dto)
    {
        try
        {
            await _clientService.UpdateCompanyClientAsync(dto);
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
