namespace Project.DTOs.Client;

public abstract class UpdateClientDto
{
    public string? Address { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
}