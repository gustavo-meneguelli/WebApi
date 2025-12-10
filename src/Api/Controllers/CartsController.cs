using Application.Common.Models;
using Application.Features.Carts.DTOs;
using Application.Features.Carts.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartsController(
    ICartService cartService,
    IValidator<AddToCartDto> addValidator,
    IValidator<UpdateCartItemDto> updateValidator)
    : MainController
{
    /// <summary>
    /// Obtém o carrinho do usuário logado.
    /// </summary>
    /// <returns>Carrinho com items, preços congelados vs atuais, e economia total.</returns>
    /// <response code="200">Carrinho retornado (pode estar vazio).</response>
    [HttpGet]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyCartAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await cartService.GetMyCartAsync(userId);
        return ParseResult(result);
    }

    /// <summary>
    /// Adiciona um produto ao carrinho (ou atualiza quantidade se já existir).
    /// </summary>
    /// <param name="item">ProductId e Quantity</param>
    /// <returns>Carrinho atualizado</returns>
    /// <response code="200">Produto adicionado com sucesso.</response>
    /// <response code="400">Dados inválidos (ProductId ou Quantity)</response>
    /// <response code="404">Produto não encontrado</response>
    [HttpPost("items")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddItemAsync(AddToCartDto item)
    {
        var validationResult = await addValidator.ValidateAsync(item);
        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await cartService.AddItemAsync(userId, item);
        return ParseResult(result);
    }

    /// <summary>
    /// Atualiza a quantidade de um item no carrinho.
    /// </summary>
    /// <param name="cartItemId">ID do item no carrinho</param>
    /// <param name="update">Nova quantidade (1-99)</param>
    /// <returns>Carrinho atualizado</returns>
    /// <response code="200">Quantidade atualizada.</response>
    /// <response code="400">Quantidade inválida</response>
    /// <response code="404">Item não encontrado no carrinho</response>
    [HttpPut("items/{cartItemId}")]
    [ProducesResponseType(typeof(CartResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateItemQuantityAsync(int cartItemId, UpdateCartItemDto update)
    {
        var validationResult = await updateValidator.ValidateAsync(update);
        var errorResponse = CustomResponse(validationResult);
        if (errorResponse is not null) return errorResponse;

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await cartService.UpdateItemQuantityAsync(userId, cartItemId, update.Quantity);
        return ParseResult(result);
    }

    /// <summary>
    /// Remove um item do carrinho.
    /// </summary>
    /// <param name="cartItemId">ID do item no carrinho</param>
    /// <returns>Mensagem de sucesso</returns>
    /// <response code="200">Item removido</response>
    /// <response code="404">Item não encontrado</response>
    [HttpDelete("items/{cartItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItemAsync(int cartItemId)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await cartService.RemoveItemAsync(userId, cartItemId);
        return ParseResult(result);
    }

    /// <summary>
    /// Limpa todos os items do carrinho.
    /// </summary>
    /// <returns>Mensagem de sucesso</returns>
    /// <response code="200">Carrinho limpo</response>
    /// <response code="404">Carrinho não encontrado</response>
    [HttpDelete("clear")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ClearCartAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await cartService.ClearCartAsync(userId);
        return ParseResult(result);
    }
}
