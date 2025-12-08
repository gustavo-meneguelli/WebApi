using Application.Common.Models;
using Application.DTO.Products;
using Application.Interfaces.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController(
    IProductService service,
    IValidator<CreateProductDto> createValidator,
    IValidator<UpdateProductDto> updateValidator)
    : MainController
{
    /// <summary>
    /// Recupera uma lista paginada de produtos.
    /// </summary>
    /// <param name="paginationParams">Parâmetros de paginação (número da página e tamanho).</param>
    /// <returns>Retorna um objeto PagedResult contendo a lista e metadados.</returns>
    /// <response code="200">Retorna a lista de produtos (pode estar vazia).</response>
    /// <response code="401">Usuário não autenticado.</response>
    [Authorize]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
    {
        var result = await service.GetAllAsync(paginationParams);
        
        return ParseResult(result);
    }

    /// <summary>
    /// Busca um produto específico pelo ID.
    /// </summary>
    /// <param name="id">ID do produto.</param>
    /// <returns>Os detalhes do produto.</returns>
    /// <response code="200">Produto encontrado.</response>
    /// <response code="404">Nenhum produto encontrado com este ID.</response>
    [Authorize]
    [HttpGet("{id}"), ActionName("GetByIdAsync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var result = await service.GetByIdAsync(id);

        return ParseResult(result);
    }

    /// <summary>
    /// Cria um produto no catálogo.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// </remarks>
    /// <param name="dto">Dados do produto a ser criado.</param>
    /// <returns>Retorna o produto recém-criado.</returns>
    /// <response code="201">Produto criado com sucesso.</response>
    /// <response code="400">Dados inválidos (ex: Preço negativo, Nome curto).</response>
    /// <response code="409">Já existe um produto com este nome.</response>
    /// <response code="401">Não autenticado.</response>
    /// <response code="403">Não autorizado (apenas Admins).</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddAsync(CreateProductDto dto)
    {
        var validationResult = await createValidator.ValidateAsync(dto);
        
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;
        
        var result = await service.AddAsync(dto);

        return ParseResult(result);
    }

    /// <summary>
    /// Atualiza os dados de um produto existente.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// A atualização valida se o novo nome já existe em outro produto para evitar duplicatas.
    /// </remarks>
    /// <param name="id">ID do produto a ser atualizado.</param>
    /// <param name="dto">Novos dados do produto (Nome e/ou Preço).</param>
    /// <returns>O produto atualizado.</returns>
    /// <response code="200">Produto atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos fornecidos.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins).</response>
    /// <response code="404">Nenhum produto encontrado com este ID.</response>
    /// <response code="409">Já existe outro produto com este nome.</response>
    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProductDto dto)
    {
        var validationResult = await updateValidator.ValidateAsync(dto);
        var validationResponse = CustomResponse(validationResult);
        if (validationResponse is not null) return validationResponse;
        
        var result = await service.UpdateAsync(id, dto);

        return ParseResult(result);
    }

    /// <summary>
    /// Remove um produto do catálogo.
    /// </summary>
    /// <remarks>
    /// Requer permissão de **Admin**.
    /// Essa operação é irreversível.
    /// </remarks>
    /// <param name="id">ID do produto a ser removido.</param>
    /// <response code="204">Produto removido com sucesso.</response>
    /// <response code="401">Usuário não autenticado.</response>
    /// <response code="403">Acesso negado (apenas Admins).</response>
    /// <response code="404">Nenhum produto encontrado com este ID.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var result = await service.DeleteAsync(id);

        return ParseResult(result);
    }
}