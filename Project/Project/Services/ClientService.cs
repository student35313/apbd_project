using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Client;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class ClientService : IClientService
{
    private readonly DatabaseContext _context;

    public ClientService(DatabaseContext context)
    {
        _context = context;
    }

    public async Task AddIndividualClientAsync(AddIndividualClientDto dto)
    {
        var exists = await _context.Clients
            .OfType<IndividualClient>()
            .AnyAsync(c => c.Pesel == dto.Pesel);

        if (exists)
            throw new ConflictException("Client with this PESEL already exists.");

        var emailExists = await _context.Clients.AnyAsync(c => c.Email == dto.Email);
        if (emailExists)
            throw new ConflictException("Client with this email already exists.");

        var phoneExists = await _context.Clients.AnyAsync(c => c.PhoneNumber == dto.PhoneNumber);
        if (phoneExists)
            throw new ConflictException("Client with this phone number already exists.");

        var client = new IndividualClient
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Pesel = dto.Pesel,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            IsDeleted = false
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
    }

    public async Task AddCompanyClientAsync(AddCompanyClientDto dto)
    {
        var exists = await _context.Clients
            .OfType<CompanyClient>()
            .AnyAsync(c => c.KrsNumber == dto.KrsNumber);

        if (exists)
            throw new ConflictException("Company with this KRS already exists.");

        var emailExists = await _context.Clients.AnyAsync(c => c.Email == dto.Email);
        if (emailExists)
            throw new ConflictException("Client with this email already exists.");

        var phoneExists = await _context.Clients.AnyAsync(c => c.PhoneNumber == dto.PhoneNumber);
        if (phoneExists)
            throw new ConflictException("Client with this phone number already exists.");

        var client = new CompanyClient
        {
            CompanyName = dto.CompanyName,
            KrsNumber = dto.KrsNumber,
            Address = dto.Address,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber
        };

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
    }

    
    public async Task RemoveClientByPeselAsync(string pesel)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var client = await _context.Clients
                .OfType<IndividualClient>()
                .Include(c => c.Products)
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.Pesel == pesel);

            if (client == null)
                throw new NotFoundException("Client with given PESEL not found.");
            
            _context.Products.RemoveRange(client.Products);
            
            _context.Contracts.RemoveRange(client.Contracts);

            client.FirstName = "REMOVED";
            client.LastName = "REMOVED";
            client.Address = "REMOVED";
            client.Email = "REMOVED";
            client.PhoneNumber = "REMOVED";
            client.IsDeleted = true;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task UpdateIndividualClientAsync(UpdateIndividualClientDto dto)
    {
        var client = await _context.Clients
            .OfType<IndividualClient>()
            .FirstOrDefaultAsync(c => c.Pesel == dto.Pesel && !c.IsDeleted);

        if (client == null)
            throw new NotFoundException("Individual client not found.");
        
        if (!string.IsNullOrWhiteSpace(dto.FirstName))
            client.FirstName = dto.FirstName;
        if (!string.IsNullOrWhiteSpace(dto.LastName))
            client.LastName = dto.LastName;
        if (!string.IsNullOrWhiteSpace(dto.Address))
            client.Address = dto.Address;
        if (!string.IsNullOrWhiteSpace(dto.Email))
            client.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            client.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
    }
    
    public async Task UpdateCompanyClientAsync(UpdateCompanyClientDto dto)
    {
        var client = await _context.Clients
            .OfType<CompanyClient>()
            .FirstOrDefaultAsync(c => c.KrsNumber == dto.KrsNumber);

        if (client == null)
            throw new NotFoundException("Company client not found.");
        
        if (!string.IsNullOrWhiteSpace(dto.CompanyName))
            client.CompanyName = dto.CompanyName;
        if (!string.IsNullOrWhiteSpace(dto.Address))
            client.Address = dto.Address;
        if (!string.IsNullOrWhiteSpace(dto.Email))
            client.Email = dto.Email;
        if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
            client.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
    }


}
