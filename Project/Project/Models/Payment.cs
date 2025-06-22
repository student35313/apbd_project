using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;
[Table("Payment")]
public class Payment
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(Contract))]
    public int ContractId { get; set; }
    
    public virtual Contract Contract { get; set; } = null!;

    public DateTime Date { get; set; }
    [Precision(10,2)]
    public decimal Amount { get; set; }
}
