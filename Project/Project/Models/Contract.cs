using Microsoft.EntityFrameworkCore;

namespace Project.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Contract")]
public class Contract
{
    [Key]
    public int Id { get; set; }
    [ForeignKey(nameof(Client))]
    public int? ClientId { get; set; }
    public virtual Client? Client { get; set; } = null!;
    [ForeignKey(nameof(SoftwareProduct))]
    public int SoftwareProductId { get; set; }
    
    public virtual SoftwareProduct Product { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    
    [Precision(10,2)]
    public decimal FinalPrice { get; set; }
    [Range(0,3)]
    public int ExtraSupportYears { get; set; }
    public bool IsSigned { get; set; }
    public bool IsCancelled { get; set; }

    public virtual ICollection<Payment> Payments { get; set; }
}
