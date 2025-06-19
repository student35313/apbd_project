using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public class UpdateIndividualClientDto : UpdateClientDto
{
    [Required]
    public string Pesel { get; set; } = null!;

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}