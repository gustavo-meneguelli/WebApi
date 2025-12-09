using Application.Common.Models;
using Application.Features.Categories.DTOs;
using Application.Features.Categories.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController(
    ICategoryService service,
    IValidator<CreateCategoryDto> createValidator,
    IValidator<UpdateCategoryDto> updateValidator)
    : MainController
{
    /// <summary>
    /// Recupera uma lista paginada de categorias.
    /// </summary>
    /// <param name="paginationParams">Parâmetros de paginação (pageNumber e pageSize).</param>
    /// <returns>Retorna um objeto PagedResult contendo a lista e metadados.</returns>
    /// <response code="200">Retorna a lista de categorias (pode estar vazia).</response>
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
    /// <param name="category">Dados da nova categoria (Nome).</param>
    /// <returns>A categoria recém-criada.</returns>
    /// <response code="201">Categoria criada com sucesso.</response>
    /// <response code="400">Dados inválidos (ex: Nome vazio ou curto demais).</response>
    /// <response code="409">Já existe uma categoria com este nome.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins podem criar).</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryDto category)
    {
        var validationResult = await createValidator.ValidateAsync(category);
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;

        var result = await service.AddAsync(category);

        return ParseResult(result);
    }

    /// <summary>
    /// Atualiza os dados de uma categoria existente.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// 
    /// **Update Parcial:** Campos vazios são ignorados (não serão atualizados).
    /// </remarks>
    /// <param name="id">ID da categoria a ser atualizada.</param>
    /// <param name="updates">Novos dados da categoria.</param>
    /// <returns>A categoria atualizada.</returns>
    /// <response code="200">Categoria atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos.</response>
    /// <response code="404">Categoria não encontrada.</response>
    /// <response code="409">Já existe outra categoria com este nome.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins).</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateCategoryDto updates)
    {
        var validationResult = await updateValidator.ValidateAsync(updates);
        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var result = await service.UpdateAsync(id, updates);

        return ParseResult(result);
    }

    /// <summary>
    /// Remove uma categoria do sistema.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// 
    /// **Atenção:** Não é possível deletar categoria que possui produtos vinculados 
    /// (proteção de integridade referencial).
    /// </remarks>
    /// <param name="id">ID da categoria a ser removida.</param>
    /// <response code="204">Categoria removida com sucesso.</response>
    /// <response code="400">Categoria possui produtos vinculados.</response>
    /// <response code="404">Categoria não encontrada.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins).</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await service.DeleteAsync(id);

        return ParseResult(result);
    }
}
