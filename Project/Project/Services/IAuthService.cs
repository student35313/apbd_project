using Project.DTOs;

namespace Project.Services;

public interface IAuthService
{
    Task RegisterAsync(AuthRequest request);
    Task<TokensResponse> LoginAsync(AuthRequest request);
    Task<TokensResponse> RefreshTokenAsync(RefreshTokenRequest request);
}
