using Application.DTO;
using Application.Utilities;
using Domain.Models;

namespace Application.Interfaces;

public interface IProductService
{
    Result<IEnumerable<Product>> GetAll();
    Result<Product?> GetById(int id);
    Result<Product> AddProduct(CreateProductDto dto);
    Result<Product?> UpdateProduct(int id, UpdateProductDto dto);
}