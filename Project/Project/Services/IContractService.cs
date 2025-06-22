using Project.DTOs.Contract;

namespace Project.Services;

public interface IContractService
{
    Task<decimal> CreateUpfrontContractAsync(CreateContractDto dto);
    Task SignContractAsync(SignContractDto dto);
}