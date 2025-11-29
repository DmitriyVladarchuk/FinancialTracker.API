using FinancialTracker.DTOs.AuthDtos;
using FinancialTracker.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace FinancialTracker.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterRequestDto registerDto)
    {
        try
        {
            var result = await authService.RegisterAsync(registerDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Ошибка регистрации: {Message}", ex.Message);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при регистрации");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при регистрации" });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginRequestDto loginDto)
    {
        try
        {
            var result = await authService.LoginAsync(loginDto);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning("Ошибка входа: {Message}", ex.Message);
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при входе");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при входе" });
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<AuthResponseDto>> Refresh(RefreshTokenRequestDto refreshDto)
    {
        try
        {
            var result = await authService.RefreshTokenAsync(refreshDto.RefreshToken);
            return Ok(result);
        }
        catch (SecurityTokenException ex)
        {
            logger.LogWarning("Невалидный refresh token: {Message}", ex.Message);
            return Unauthorized(new { message = "Невалидный refresh token" });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка при обновлении токена");
            return StatusCode(500, new { message = "Внутренняя ошибка сервера при обновлении токена" });
        }
    }
}