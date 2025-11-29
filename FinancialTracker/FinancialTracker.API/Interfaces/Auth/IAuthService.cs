using FinancialTracker.DTOs.AuthDtos;

namespace FinancialTracker.Interfaces.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task RevokeRefreshTokenAsync(string refreshToken);
}