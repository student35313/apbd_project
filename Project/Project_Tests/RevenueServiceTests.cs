using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Data;
using Project.DTOs.Client;
using Project.Models;
using Project.Services;

namespace Project_Tests
{
    public class RevenueServiceTests
    {
        private readonly DatabaseContext _context;
        private readonly RevenueService _revenueService;

        public RevenueServiceTests()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseSqlite("Filename=:memory:")
                .Options;

            _context = new DatabaseContext(options);
            _context.Database.OpenConnection();
            _context.Database.EnsureCreated();
            
            var handler = new MyHttpMessageHandler();
            var httpClient = new HttpClient(handler);

            var exchangeRateService = new ExchangeRateService(httpClient);
            _revenueService = new RevenueService(_context, exchangeRateService);
        }
        
        [Fact]
        public async Task GetRevenueForClientAsync()
        {
            var clientId = await CreateData(false);

            var dto = new RevenueRequestDto
            {
                ClientId = clientId,
                Currency = "PLN",
                Predicted = false
            };
            
            var result = await _revenueService.GetRevenueForClientAsync(dto);
            
            Assert.Equal(600m, result);
            
            var dto2 = new RevenueRequestDto
            {
                ClientId = clientId,
                Currency = "USD",
                Predicted = false
            };
            
            var result2 = await _revenueService.GetRevenueForClientAsync(dto2);
            
            Assert.Equal(120m, result2);
        }
        
        [Fact]
        public async Task GetRevenueForProductAsync()
        {
            var productId = await CreateData(true);
            
            var dto = new RevenueRequestDto
            {
                ProductId = productId,
                Currency = "PLN",
                Predicted = false
            };
            
            var result = await _revenueService.GetRevenueForProductAsync(dto);
            
            Assert.Equal(600m, result);
            
            var dto2 = new RevenueRequestDto
            {
                ProductId = productId,
                Currency = "USD",
                Predicted = true
            };
            
            var result2 = await _revenueService.GetRevenueForProductAsync(dto2);
            Assert.Equal(200m, result2);
        }

        private async Task<int> CreateData(bool isProduct)
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

            var payment = new Payment
            {
                ContractId = contract.Id,
                Amount = 600m,
                Date = DateTime.Now
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            if (isProduct) return product.Id;
            return client.Id;
        }
        
        
        
    }

    public class MyHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var json = """
                       {
                           "rates": [
                               { "mid": 5.00 }
                           ]
                       }
                       """;

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }
}