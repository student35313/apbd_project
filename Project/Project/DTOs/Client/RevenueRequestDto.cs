using System.ComponentModel.DataAnnotations;

namespace Project.DTOs.Client;

public class RevenueRequestDto
{
    [Required]
    public string Currency { get; set; } = "PLN";
    public int? ClientId { get; set; }
    public int? ProductId { get; set; }
    [Required]
    public bool Predicted { get; set; }
}