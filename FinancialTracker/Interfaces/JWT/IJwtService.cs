using System.Security.Claims;
using FinancialTracker.Data.Entities;

namespace FinancialTracker.Interfaces.JWT;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}