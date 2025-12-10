using Application.Common.Models;
using Application.Features.Orders.DTOs;
using Application.Features.Orders.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController(IOrderService orderService) : MainController
{
    /// <summary>
    /// Cria um pedido a partir do carrinho (checkout).
    /// </summary>
    /// <returns>Pedido criado com OrderNumber único</returns>
    /// <response code="200">Pedido criado. Carrinho limpo automaticamente.</response>
    /// <response code="404">Carrinho vazio</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateFromCartAsync()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await orderService.CreateFromCartAsync(userId);
        return ParseResult(result);
    }

    /// <summary>
    /// Obtém um pedido específico por ID.
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Detalhes do pedido</returns>
    /// <response code="200">Pedido encontrado</response>
    /// <response code="404">Pedido não encontrado</response>
    /// <response code="401">Usuário não é dono do pedido</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await orderService.GetByIdAsync(id, userId);
        return ParseResult(result);
    }

    /// <summary>
    /// Lista todos os pedidos do usuário logado (paginado).
    /// </summary>
    /// <param name="pageNumber">Número da página (padrão: 1)</param>
    /// <param name="pageSize">Itens por página (padrão: 10)</param>
    /// <returns>Lista paginada de pedidos</returns>
    /// <response code="200">Pedidos encontrados</response>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<OrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyOrdersAsync([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var pagination = new PaginationParams { PageNumber = pageNumber, PageSize = pageSize };
        var result = await orderService.GetMyOrdersAsync(userId, pagination);
        return ParseResult(result);
    }

    /// <summary>
    /// Cancela um pedido (apenas se estiver Pending).
    /// </summary>
    /// <param name="id">ID do pedido</param>
    /// <returns>Pedido cancelado</returns>
    /// <response code="200">Pedido cancelado</response>
    /// <response code="404">Pedido não encontrado ou não pode ser cancelado</response>
    /// <response code="401">Usuário não é dono do pedido</response>
    [HttpPatch("{id}/cancel")]
    [ProducesResponseType(typeof(OrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CancelOrderAsync(int id)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await orderService.CancelOrderAsync(id, userId);
        return ParseResult(result);
    }
}
