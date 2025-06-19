using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public abstract class AddClientDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = null!;
    
    [Required]
    public string Address { get; set; } = null!;
}
