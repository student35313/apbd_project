using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

[Table("Client")]
[Index(nameof(Email),  IsUnique = true)]
[Index(nameof(PhoneNumber),  IsUnique = true)]
public abstract class Client
{
    [Key] 
    public int Id { get; set; }

    [MaxLength(200)]
    public string Address { get; set; } = null!;

    [EmailAddress, MaxLength(100)]
    public string Email { get; set; } = null!;

    [Phone, MaxLength(30)]
    public string PhoneNumber { get; set; } = null!;
    
    public bool IsDeleted { get; set; }
    
    public void MarkAsDeleted() => IsDeleted = true;
    
    public virtual ICollection<Product> Products { get; set; }
    
    public virtual ICollection<Contract> Contracts { get; set; }


}
