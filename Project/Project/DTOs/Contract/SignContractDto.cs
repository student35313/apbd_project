using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Contract;

public class SignContractDto
{
    [Required]
    public int ClientId { get; set; }
    [Required]
    public int SoftwareProductId { get; set; }
}