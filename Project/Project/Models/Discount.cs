using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;
[Table("Discount")]
public class Discount
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Name { get; set; } = null!;

    public bool AppliesToSubscription { get; set; }
    public bool AppliesToUpfront { get; set; }

    [Range(0, 100), Precision(5,2)]
    public decimal Percentage { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Product> Products { get; set; }
}
