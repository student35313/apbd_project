using Project.DTOs.Contract;

namespace Project.Services;

public interface IContractService
{
    Task<decimal> CreateUpfrontContractAsync(CreateContractDto dto);
    Task DeleteContractAsync(int contractId);
    Task<decimal> AddPaymentAsync(AddPaymentDto dto);

}