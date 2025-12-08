using Application.Common.Models;
using Application.DTO.Categories;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ICategoryService service,
    IValidator<CreateCategoryDto> validator)
    : MainController
{
    /// <summary>
    /// Recupera a lista completa de categorias.
    /// </summary>
    /// <returns>Uma lista contendo Id e Nome das categorias.</returns>
    /// <response code="200">Retorna a lista de categorias com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
    {
        var result = await service.GetAllAsync(paginationParams);
        
        return ParseResult(result);
    }
    
    /// <summary>
    /// Busca uma categoria específica pelo ID.
    /// </summary>
    /// <param name="id">O ID da categoria.</param>
    /// <returns>Os dados da categoria solicitada.</returns>
    /// <response code="200">Categoria encontrada.</response>
    /// <response code="404">Nenhuma categoria encontrada com este ID.</response>
    /// <response code="401">Usuário não autenticado.</response>
    [HttpGet("{id}"), ActionName("GetByIdAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await service.GetByIdAsync(id);

        return ParseResult(result);
    }

    /// <summary>
    /// Cria uma categoria no sistema.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// </remarks>
    /// <param name="dto">Dados da nova categoria (Nome).</param>
    /// <returns>A categoria recém-criada.</returns>
    /// <response code="201">Categoria criada com sucesso.</response>
    /// <response code="400">Dados inválidos (ex:Nome vazio ou curto demais).</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins podem criar).</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryDto dto)
    {
        var validationResult = await validator.ValidateAsync(dto);
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;
        
        var result = await service.AddAsync(dto);
        
        return ParseResult(result);
    }
}