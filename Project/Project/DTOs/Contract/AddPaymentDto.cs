using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Contract;

public class AddPaymentDto
{
    [Required]
    public int ContractId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Amount { get; set; }
}
