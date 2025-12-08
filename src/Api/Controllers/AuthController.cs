using Application.DTO.Auth;
using Application.Interfaces.Security;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IValidator<LoginDto> loginValidator,
    IValidator<UserRegisterDto> registerValidator)
    : MainController
{
    /// <summary>
    /// Realiza o login de um usuário existente.
    /// </summary>
    /// <param name="dto">Objeto contendo nome de usuário e senha.</param>
    /// <returns>Retorna um token JWT se as credenciais forem válidas.</returns>
    /// <response code="200">Login realizado com sucesso. Retorna o Token.</response>
    /// <response code="401">Usuário ou senha inválidos.</response>
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var validationResult = await loginValidator.ValidateAsync(dto);
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;
        
        var result = await authService.LoginAsync(dto);

        return ParseResult(result);
    }

    /// <summary>
    /// Registra um novo usuário comum no sistema.
    /// </summary>
    /// <remarks>
    /// O usuário criado terá automaticamente o perfil de **Common**.
    /// </remarks>
    /// <param name="dto">Dados para registro (Nome de usuário e Senha).</param>
    /// <returns>Mensagem de sucesso.</returns>
    /// <response code="200">Usuário registrado com sucesso.</response>
    /// <response code="400">Dados inválidos (ex: senha vazia).</response>
    /// <response code="409">Nome de usuário já está em uso.</response>
    [HttpPost("Register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
    {
        var validationResult = await registerValidator.ValidateAsync(dto);
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;
        
        var result = await authService.RegisterAsync(dto);
        
        return ParseResult(result);
    }
}