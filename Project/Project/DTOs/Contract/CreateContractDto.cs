using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Contract;

public class CreateContractDto
{
    [Required]
    public int ClientId { get; set; }
    [Required]
    public int SoftwareProductId { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public DateTime EndDate { get; set; }
    [Required]
    [Range(0,3)]
    public int ExtraSupportYears { get; set; }
}
