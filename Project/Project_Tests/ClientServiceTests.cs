using Project.Models;

namespace Project_Tests;

using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Client;
using Project.Exceptions;
using Project.Services;
using Xunit;


public class ClientServiceTests
{
    private static DatabaseContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new DatabaseContext(options);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldAddClient_WhenDataIsValid()
    {
        // Arrange
        var context = GetInMemoryContext();
        var service = new ClientService(context);
        var dto = new AddIndividualClientDto
        {
            FirstName = "John",
            LastName = "Doe",
            Address = "123 Street",
            Email = "john@example.com",
            PhoneNumber = "123456789",
            Pesel = "12345678901"
        };

        // Act
        await service.AddIndividualClientAsync(dto);

        // Assert
        var client = await context.Clients.OfType<IndividualClient>().FirstOrDefaultAsync();
        Assert.NotNull(client);
        Assert.Equal("John", client!.FirstName);
    }

    [Fact]
    public async Task AddIndividualClientAsync_ShouldThrow_WhenPeselExists()
    {
        var context = GetInMemoryContext();
        var service = new ClientService(context);

        var dto = new AddIndividualClientDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Address = "456 Street",
            Email = "jane@example.com",
            PhoneNumber = "987654321",
            Pesel = "99999999999"
        };

        await service.AddIndividualClientAsync(dto);

        // Act + Assert
        await Assert.ThrowsAsync<ConflictException>(() => service.AddIndividualClientAsync(dto));
    }
}
