namespace FinancialTracker.DTOs.AuthDtos;

public class AuthResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
    public DateTime AccessTokenExpires { get; set; }
    public DateTime RefreshTokenExpires { get; set; }
}