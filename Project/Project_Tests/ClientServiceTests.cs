using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Client;
using Project.Exceptions;
using Project.Models;
using Project.Services;
using Xunit;

namespace Project_Tests;

public class ClientServiceTests
{
    private readonly DatabaseContext _context;
    private readonly ClientService _service;

    public ClientServiceTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.EnsureCreated();

        _service = new ClientService(_context);
    }
    
    
    [Fact]
    public async Task AddIndividualClient()
    {
        var dto = new AddIndividualClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Pesel = "12345678901",
            Email = "john@example.com",
            PhoneNumber = "+48123456789",
            Address = "123 Main St"
        };

        await _service.AddIndividualClientAsync(dto);

        var client = await _context.Clients.OfType<IndividualClient>().FirstOrDefaultAsync(c => c.Pesel == dto.Pesel);
        Assert.NotNull(client);
        Assert.Equal(dto.Email, client.Email);
        
        await Assert.ThrowsAsync<ConflictException>(() => _service.AddIndividualClientAsync(dto));
    }

    [Fact]
    public async Task AddCompanyClientAsync()
    {
        var dto = new AddCompanyClientDto
        {
            CompanyName = "Test Co",
            KrsNumber = "1234567890",
            Email = "company@test.com",
            PhoneNumber = "999888777",
            Address = "Warsaw"
        };

        await _service.AddCompanyClientAsync(dto);

        var exists = await _context.Clients
            .OfType<Project.Models.CompanyClient>()
            .AnyAsync(c => c.KrsNumber == dto.KrsNumber);

        Assert.True(exists);
        
        await Assert.ThrowsAsync<ConflictException>(() => _service.AddCompanyClientAsync(dto));
    }
    
    
    [Fact]
    public async Task RemoveClientByPeselAsync()
    {
        var client = new IndividualClient
        {
            FirstName = "Anna",
            LastName = "Smith",
            Pesel = "99999999999",
            Address = "Warsaw",
            Email = "anna@test.com",
            PhoneNumber = "123123123"
        };

        _context.Clients.Add(client);
        
        await _context.SaveChangesAsync();

        await _service.RemoveClientByPeselAsync("99999999999");

        var clientAfter = await _context.Clients
            .OfType<IndividualClient>()
            .FirstOrDefaultAsync(c => c.Pesel == "99999999999");

        Assert.NotNull(clientAfter);
        Assert.True(clientAfter.IsDeleted);
    }

    [Fact]
    public async Task RemoveClientByPeselAsync_WhenClientNotFound()
    {
        await Assert.ThrowsAsync<NotFoundException>(() =>
            _service.RemoveClientByPeselAsync("nonexistent"));
    }
    
    [Fact]
    public async Task UpdateIndividualClientAsync()
    {
        var client = new IndividualClient
        {
            FirstName = "Jan",
            LastName = "Kowalski",
            Pesel = "12345678901",
            Address = "Old",
            Email = "old@example.com",
            PhoneNumber = "111222333"
        };
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();

        var dto = new UpdateIndividualClientDto
        {
            Pesel = "12345678901",
            FirstName = "Adam",
            Address = "New Street 5"
        };

        await _service.UpdateIndividualClientAsync(dto);

        var updated = await _context.Clients.OfType<IndividualClient>()
            .FirstOrDefaultAsync(c => c.Pesel == "12345678901");

        Assert.Equal("Adam", updated!.FirstName);
        Assert.Equal("New Street 5", updated.Address);
        Assert.Equal("Kowalski", updated.LastName);
    }
    
    [Fact]
    public async Task UpdateCompanyClientAsync()
    {
        var company = new CompanyClient
        {
            CompanyName = "OldName",
            KrsNumber = "9876543210",
            Address = "Old Address",
            Email = "company@old.com",
            PhoneNumber = "999888777"
        };
        _context.Clients.Add(company);
        await _context.SaveChangesAsync();

        var dto = new UpdateCompanyClientDto
        {
            KrsNumber = "9876543210",
            CompanyName = "NewName"
        };

        await _service.UpdateCompanyClientAsync(dto);

        var updated = await _context.Clients.OfType<CompanyClient>()
            .FirstOrDefaultAsync(c => c.KrsNumber == "9876543210");

        Assert.Equal("NewName", updated!.CompanyName);
        Assert.Equal("Old Address", updated.Address);
    }
}