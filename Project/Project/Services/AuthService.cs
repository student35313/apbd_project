using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Project.Data;
using Project.DTOs;
using Project.Exceptions;
using Project.Models;

namespace Project.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;
    private readonly DatabaseContext _context;
    
    public AuthService(IConfiguration config, DatabaseContext context)
    {
        _config = config;
        _context = context;
    }
    
    
    public async Task RegisterAsync(AuthRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            throw new ConflictException("Username already taken.");

        var user = new User
        {
            Username = request.Username,
            PasswordHash = new PasswordHasher<User>().HashPassword(new User(), request.Password),
            RoleId = 1 // Default role: User
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<TokensResponse> LoginAsync(AuthRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var user = await _context.Users.Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (user == null)
                throw new NotFoundException("User not found.");

            var result = new PasswordHasher<User>()
                .VerifyHashedPassword(new User(), user.PasswordHash, request.Password);

            if (result != PasswordVerificationResult.Success)
                throw new NotFoundException("Invalid credentials.");

            var accessToken = CreateAccessToken(user);
            var refreshToken = CreateRefreshToken();

            // Вот тут проверка — если токен уже есть, мы его обновляем:
            var existingToken = await _context.RefreshTokens.FindAsync(user.Id);
            if (existingToken != null)
            {
                existingToken.Token = refreshToken;
                existingToken.ExpiresAt = DateTime.Now.AddMinutes(30);
                _context.RefreshTokens.Update(existingToken);
            }
            else
            {
                _context.RefreshTokens.Add(new RefreshToken
                {
                    UserId = user.Id,
                    Token = refreshToken,
                    ExpiresAt = DateTime.Now.AddMinutes(30)
                });
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new TokensResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }


    public async Task<TokensResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
        var token = await _context.RefreshTokens
            .Include(r => r.User)
            .ThenInclude(u => u.UserRole)
            .FirstOrDefaultAsync(r => r.Token == request.RefreshToken);

        if (token == null || token.ExpiresAt < DateTime.Now)
            throw new NotFoundException("Invalid or expired refresh token.");

        var newAccessToken = CreateAccessToken(token.User);
        var newRefreshToken = CreateRefreshToken();

        token.Token = newRefreshToken;
        token.ExpiresAt = DateTime.Now.AddMinutes(30);
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return new TokensResponse { AccessToken = newAccessToken, RefreshToken = newRefreshToken };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private string CreateAccessToken(User user)
    {
        
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.UserRole.Name!)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SymmetricSecurityKey"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        var token = new JwtSecurityToken(
            issuer: _config["JWT:Issuer"],
            audience: _config["JWT:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string CreateRefreshToken()
    {
        var bytes = new byte[96];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }
}
