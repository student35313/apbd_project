using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Project.Models;

[Table("User_")]
[Index(nameof(Username), IsUnique = true)]
public class User
{
    [Key] public int Id { get; set; }

    [MaxLength(30)] public string Username { get; set; } = null!;

    [MaxLength(256)] public string PasswordHash { get; set; } = null!;

    public int RoleId { get; set; }

    [ForeignKey(nameof(RoleId))] public virtual UserRole UserRole { get; set; } = null!;
    public virtual RefreshToken? RefreshToken { get; set; }
}
