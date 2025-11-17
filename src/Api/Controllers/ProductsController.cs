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
        return Ok(serviceResult);
    }

    [HttpGet("{id}")]
    public IActionResult GetProductById(int id)
    {
        var serviceResult = service.GetById(id);

        if (serviceResult is null)
        {
            return NotFound();
        }
        
        return Ok(serviceResult);
    }

    [HttpPost]
    public IActionResult AddProduct(string productName, int productPrice)
    {
        var  serviceResult = service.AddProduct(productName, productPrice);
        
        return CreatedAtAction(nameof(GetProductById), new { id = serviceResult.Id }, serviceResult);
    }
}