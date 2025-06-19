using Project.DTOs.Client;

namespace Project.Services;

public interface IClientService
{
    Task AddIndividualClientAsync(AddIndividualClientDto dto);
    Task AddCompanyClientAsync(AddCompanyClientDto dto);
    Task RemoveClientByPeselAsync(string pesel);
    Task UpdateIndividualClientAsync(UpdateIndividualClientDto dto);
    Task UpdateCompanyClientAsync(UpdateCompanyClientDto dto);
}
