using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Application.DTO;

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

        var token = responseContent; 
    
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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
        
        var newProduct = new CreateProductDto
        {
            Name = "Teclado Mecânico",
            Price = 150.00m
        };

        // 2. ACT 
        var productHttpResponse = await _client.PostAsJsonAsync("/api/products", newProduct);
        
        var returnedProduct = await productHttpResponse.Content.ReadFromJsonAsync<Domain.Models.Product>();

        // 3. ASSERT
        Assert.Equal(HttpStatusCode.Created, productHttpResponse.StatusCode);
        Assert.NotNull(returnedProduct);
        Assert.Equal("Teclado Mecânico", returnedProduct.Name);
        Assert.True(returnedProduct.Id > 0);
    }
}