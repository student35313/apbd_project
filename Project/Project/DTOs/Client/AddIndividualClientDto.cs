using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public class AddIndividualClientDto : AddClientDto
{
    [Required]
    [MaxLength(50)]
    public string FirstName { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string LastName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "Invalid PESEL")]
    public string Pesel { get; set; } = null!;
    
}
