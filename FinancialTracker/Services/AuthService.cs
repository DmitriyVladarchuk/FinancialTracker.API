using System.Security.Claims;
using FinancialTracker.Data;
using FinancialTracker.Data.Entities;
using FinancialTracker.DTOs.AuthDtos;
using FinancialTracker.Interfaces.Auth;
using FinancialTracker.Interfaces.JWT;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FinancialTracker.Services;

public class AuthService(FinancialDbContext context, IJwtService jwtService, IPasswordHasher passwordHasher)
    : IAuthService
{
    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto registerDto)
    {
        if (await context.Users.AnyAsync(u => u.Email == registerDto.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = passwordHasher.HashPassword(registerDto.Password),
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName
        };

        context.Users.Add(user);
        await context.SaveChangesAsync();

        await CreateDefaultCategoriesForUser(user.Id);

        return await GenerateTokens(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto loginDto)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null || !passwordHasher.VerifyPassword(loginDto.Password, user.PasswordHash))
            throw new InvalidOperationException("Неверный email или пароль");

        await RevokeUserRefreshTokens(user.Id);

        return await GenerateTokens(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken == null)
            throw new SecurityTokenException("Refresh token не найден");

        if (storedToken.IsRevoked)
            throw new SecurityTokenException("Refresh token отозван");

        if (storedToken.Expires < DateTime.UtcNow)
            throw new SecurityTokenException("Refresh token истек");

        try
        {
            jwtService.GetPrincipalFromExpiredToken(refreshToken);
        }
        catch
        {
            storedToken.IsRevoked = true;
            await context.SaveChangesAsync();
            throw new SecurityTokenException("Неверный refresh token");
        }
        
        storedToken.IsRevoked = true;

        return await GenerateTokens(storedToken.User);
    }

    public async Task RevokeRefreshTokenAsync(string refreshToken)
    {
        var storedToken = await context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (storedToken != null && !storedToken.IsRevoked)
        {
            storedToken.IsRevoked = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task RevokeUserRefreshTokens(Guid userId)
    {
        var activeTokens = await context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked && rt.Expires > DateTime.UtcNow)
            .ToListAsync();

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
        }

        await context.SaveChangesAsync();
    }

    private async Task<AuthResponseDto> GenerateTokens(User user)
    {
        var accessToken = jwtService.GenerateAccessToken(user);
        var refreshToken = jwtService.GenerateRefreshToken();
        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(30),
            Created = DateTime.UtcNow,
            IsRevoked = false
        };

        context.RefreshTokens.Add(refreshTokenEntity);
        await context.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpires = DateTime.UtcNow.AddMinutes(5),
            RefreshTokenExpires = DateTime.UtcNow.AddDays(30),
        };
    }

    private async Task CreateDefaultCategoriesForUser(Guid userId)
    {
        var defaultCategories = new[]
        {
            new Category { Name = "Еда", UserId = userId },
            new Category { Name = "Транспорт", UserId = userId },
            new Category { Name = "Развлечения", UserId = userId },
            new Category { Name = "Зарплата", UserId = userId },
            new Category { Name = "Прочее", UserId = userId }
        };

        await context.Categories.AddRangeAsync(defaultCategories);
        await context.SaveChangesAsync();
    }
}