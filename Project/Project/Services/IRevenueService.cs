using Project.DTOs.Client;

namespace Project.Services;

public interface IRevenueService
{
    Task<decimal> GetRevenueForClientAsync(RevenueRequestDto dto);
    Task<decimal> GetRevenueForProductAsync(RevenueRequestDto dto);
}