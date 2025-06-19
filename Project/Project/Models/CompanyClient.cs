using System.ComponentModel.DataAnnotations;

namespace Project.Models;


public class CompanyClient : Client
{
    [MaxLength(120)]
    public string CompanyName { get; set; } = null!;
    
    [MaxLength(10)]
    public string KrsNumber { get; set; } = null!;

    public CompanyClient() { }
    
}
