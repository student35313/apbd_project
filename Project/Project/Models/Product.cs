using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;
[Table("Product")]
public abstract class Product
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;
    [Precision(10,2)]
    public decimal? UpfrontPrice { get; set; }
    [Precision(10,2)]
    public decimal? SubscriptionPrice { get; set; }
    [MaxLength(100)]
    public string Category { get; set; } = null!;
    
    [ForeignKey(nameof(Client))]
    public int? ClientId { get; set; }
    
    public virtual Client? Client { get; set; } = null!;

    public ICollection<Discount> Discounts { get; set; }

}
