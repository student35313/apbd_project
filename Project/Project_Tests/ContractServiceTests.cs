using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Contract;
using Project.Exceptions;
using Project.Models;
using Project.Services;

namespace Project_Tests;

public class ContractServiceTests
{
    private readonly DatabaseContext _context;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.EnsureCreated();

        _service = new ContractService(_context);
    }

    private async Task<(IndividualClient, SoftwareProduct)> CreateData()
    {
        var client = new IndividualClient
        {
            FirstName = "John",
            LastName = "Doe",
            Pesel = "12345678931",
            Email = "098@example.com",
            PhoneNumber = "123456788",
            Address = "Address 1",
            IsDeleted = false
        };

        var product = new SoftwareProduct
        {
            Name = "Test Product",
            UpfrontPrice = 1000m,
            Description = "Some description",
            Category = "Test",
            CurrentVersion = "1.0",
            Client = client
        };

        _context.Clients.Add(client);
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return (client, product);
    }

    [Fact]
    public async Task CreateUpfrontContractAsync()
    {
        var (client, product) = await CreateData();

        var dto = new CreateContractDto
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            StartDate = DateTime.Today,
            EndDate = DateTime.Today.AddDays(10),
            ExtraSupportYears = 1
        };

        var finalPrice = await _service.CreateUpfrontContractAsync(dto);

        var createdContract = await _context.Contracts.FirstOrDefaultAsync();
        Assert.NotNull(createdContract);
        Assert.Equal(client.Id, createdContract.ClientId);
        Assert.Equal(product.Id, createdContract.SoftwareProductId);
        Assert.False(createdContract.IsSigned);
        Assert.Equal(2000m, finalPrice);

        await Assert.ThrowsAsync<ConflictException>(() => _service.CreateUpfrontContractAsync(dto));
    }

    [Fact]
    public async Task DeleteContractAsync()
    {
        var (client, product) = await CreateData();

        var contract = new Contract
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10),
            IsCancelled = false,
            IsSigned = true,
            ExtraSupportYears = 0,
            FinalPrice = 1000
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        await _service.DeleteContractAsync(contract.Id);

        var exists = await _context.Contracts.AnyAsync(c => c.Id == contract.Id);
        Assert.False(exists);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.DeleteContractAsync(999));
    }

    [Fact]
    public async Task AddPaymentAsync()
    {
        var (client, product) = await CreateData();

        var contract = new Contract
        {
            ClientId = client.Id,
            SoftwareProductId = product.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10),
            IsCancelled = false,
            IsSigned = false,
            ExtraSupportYears = 0,
            FinalPrice = 1000
        };

        _context.Contracts.Add(contract);
        await _context.SaveChangesAsync();

        var dto = new AddPaymentDto
        {
            ContractId = contract.Id,
            Amount = 400m
        };

        var remaining = await _service.AddPaymentAsync(dto);

        Assert.Equal(600m, remaining);
        Assert.Single(await _context.Payments.ToListAsync());

        dto = new AddPaymentDto
        {
            ContractId = contract.Id,
            Amount = 700m
        };

        await Assert.ThrowsAsync<ConflictException>(() => _service.AddPaymentAsync(dto));
    }
}