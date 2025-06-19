using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public class UpdateCompanyClientDto : UpdateClientDto
{
    [Required]
    public string KrsNumber { get; set; } = null!;

    public string? CompanyName { get; set; }
    
}