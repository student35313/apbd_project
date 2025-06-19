using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public class AddCompanyClientDto : AddClientDto
{
    [Required]
    [MaxLength(100)]
    public string CompanyName { get; set; } = null!;

    [Required]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Invalid KRS number")]
    public string KrsNumber { get; set; } = null!;
}
