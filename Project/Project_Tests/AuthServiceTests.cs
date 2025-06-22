using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Services;

namespace Project_Tests;

public class AuthServiceTests
{
    private readonly AuthService _authService;
    private readonly DatabaseContext _context;
    private readonly IConfiguration _config;

    public AuthServiceTests()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open(); 

        var options = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(connection)
            .Options;

        _context = new DatabaseContext(options);
        _context.Database.EnsureCreated();

        var inMemorySettings = new Dictionary<string, string> {
            {"JWT:SymmetricSecurityKey", "supersecretkeythatshouldbelongenoughforhs512tokentoworkproperly123"},
            {"JWT:Issuer", "TestIssuer"},
            {"JWT:Audience", "TestAudience"}
        };

        _config = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        _authService = new AuthService(_config, _context);
    }
    
    [Fact]
    public async Task Register_NewUser()
    {
        var request = new AuthRequest
        {
            Username = "newuser",
            Password = "password123"
        };

        await _authService.RegisterAsync(request);
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == "newuser");
        Assert.NotNull(user);
    }
    
    [Fact]
    public async Task Register_ExistingUsername()
    {
        var request = new AuthRequest
        {
            Username = "duplicate",
            Password = "password123"
        };

        await _authService.RegisterAsync(request);

        await Assert.ThrowsAsync<ConflictException>(() => _authService.RegisterAsync(request));
    }
    
    [Fact]
    public async Task Login_ValidCredentials()
    {
        var request = new AuthRequest
        {
            Username = "loginuser",
            Password = "password123"
        };
        await _authService.RegisterAsync(request);

        var result = await _authService.LoginAsync(request);
        Assert.NotNull(result.AccessToken);
        Assert.NotNull(result.RefreshToken);
    }
    
    [Fact]
    public async Task Login_InvalidPassword()
    {
        var request = new AuthRequest
        {
            Username = "badlogin",
            Password = "correctpassword"
        };
        await _authService.RegisterAsync(request);

        var invalidRequest = new AuthRequest
        {
            Username = "badlogin",
            Password = "wrongpassword"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => _authService.LoginAsync(invalidRequest));
    }
}

