using Application.DTO;
using Application.Enums;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAllProducts()
    {
        var serviceResult = service.GetAll();
        
        return Ok(serviceResult.Data);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        var serviceResult = service.GetById(id);

        if (serviceResult.TypeResult is TypeResult.NotFound)
        {
            return NotFound(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }

    [HttpPost]
    public IActionResult AddProduct(CreateProductDto dto)
    {
        var serviceResult = service.AddProduct(dto);

        switch (serviceResult.TypeResult)
        {
            case TypeResult.Created when serviceResult.Data != null:
                return CreatedAtAction(nameof(GetProductById), new { id = serviceResult.Data.Id }, serviceResult.Data);
            case TypeResult.Duplicated:
                return Conflict(serviceResult.Message);
            default:
                return Problem("Something went wrong.");
        }
    }

    [HttpPatch("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] UpdateProductDto dto)
    {
        var serviceResult = service.UpdateProduct(id, dto);

        if (serviceResult.TypeResult is TypeResult.Duplicated)
        {
            return Conflict(serviceResult.Message);
        }
        
        return Ok(serviceResult.Data);
    }
}