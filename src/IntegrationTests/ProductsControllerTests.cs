using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTO.Auth;
using Application.DTO.Categories;
using Application.DTO.Products;
using Domain.Entities;

namespace IntegrationTests;

public class ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
    : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task Authenticate()
    {
        var loginDto = new LoginDto
        {
            Username = "admin",
            Password = "SenhaTeste123"
        };
        
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/Login", loginDto);
    
        var responseContent = await loginResponse.Content.ReadAsStringAsync();
        
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", responseContent);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WhenExecuted()
    {
        //ARRANGE
        await Authenticate();
        
        //ACT
        var response = await _client.GetAsync("/api/products");

        //ASSERT
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
    
    [Fact]
    public async Task Post_ShouldReturnCreated_WhenProductIsValid()
    {
        // 1. ARRANGE
        await Authenticate();

        var newCategory = new CreateCategoryDto { Name = "Tech" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);
    
        var newProduct = new CreateProductDto
        {
            Name = "Teclado Mecânico",
            Price = 150.00m,
            CategoryId = 1 
        };

        // 2. ACT 
        var productHttpResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        
        var returnedProduct = await productHttpResponse.Content.ReadFromJsonAsync<Product>();

        // 3. ASSERT
        Assert.Equal(HttpStatusCode.Created, productHttpResponse.StatusCode);
        Assert.NotNull(returnedProduct);
        Assert.Equal("Teclado Mecânico", returnedProduct.Name);
        Assert.True(returnedProduct.Id > 0);
    }
    
    [Fact]
    public async Task Post_ShouldReturnBadRequest_WhenNameIsDuplicate()
    {
        // 1. ARRANGE
        await Authenticate();
        
        var newCategory = new CreateCategoryDto { Name = "Tech" };
        var categoryResponse = await _client.PostAsJsonAsync("/api/categories", newCategory);

        // Cria o primeiro
        var product1 = new CreateProductDto { Name = "Duplicado", Price = 100, CategoryId = 1 };
        await _client.PostAsJsonAsync("/api/products", product1);

        // 2. ACT - Tenta criar de novo
        var response = await _client.PostAsJsonAsync("/api/products", product1);
    
        // 3. ASSERT
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    
        var content = await response.Content.ReadAsStringAsync();
    
        Assert.Contains("Já existe um registro", content, StringComparison.InvariantCultureIgnoreCase);
    }
}