using Application.Common.Models;
using Application.DTOs.Products;
using Application.Enums;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
    {
        var serviceResult = await service.GetAllAsync(paginationParams);
        
        return Ok(serviceResult.Data);
    }

    [Authorize]
    [HttpGet("{id}"), ActionName("GetByIdAsync")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var serviceResult = await service.GetByIdAsync(id);

        if (serviceResult.TypeResult is TypeResult.NotFound)
        {
            return NotFound(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> AddAsync(CreateProductDto dto)
    {
        var serviceResult = await service.AddAsync(dto);

        switch (serviceResult.TypeResult)
        {
            case TypeResult.Created when serviceResult.Data != null:
                return CreatedAtAction(nameof(GetByIdAsync), new { id = serviceResult.Data.Id }, serviceResult.Data);
            case TypeResult.Duplicated:
                return Conflict(serviceResult.Message);
            default:
                return Problem("Something went wrong.");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateProductDto dto)
    {
        var serviceResult = await service.UpdateAsync(id, dto);

        if (serviceResult.TypeResult is TypeResult.NotFound)
        {
            return NotFound(serviceResult.Message);
        }

        if (serviceResult.TypeResult is TypeResult.Duplicated)
        {
            return Conflict(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id)
    {
        var serviceResult = await service.DeleteAsync(id);

        if (serviceResult.TypeResult == TypeResult.NotFound)
        {
            return NotFound(serviceResult.Message);
        }
        
        return NoContent();
    }
}