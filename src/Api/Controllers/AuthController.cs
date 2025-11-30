using Application.DTOs.Auth;
using Application.Enums;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var auth = await authService.LoginAsync(dto);

        if (auth.TypeResult == TypeResult.Unauthorized)
        {
            return Unauthorized(auth.Message);
        }
        
        return Ok(auth.Data);
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var auth = await authService.RegisterAsync(dto);
        
        if (auth.TypeResult == TypeResult.Duplicated)
        {
            return Conflict(auth.Message);
        }
        
        return Ok(auth.Data);
    }
}