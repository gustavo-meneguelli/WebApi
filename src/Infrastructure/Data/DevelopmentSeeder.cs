using Application.Interfaces.Generics;
using Application.Interfaces.Repositories;
using Domain.Entities;

namespace Infrastructure.Data;

public class DevelopmentSeeder(
    ICategoryRepository categoryRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork)
{
    public async Task SeedAsync()
    {
        // Verifica se já existem categorias (evita duplicação em restarts)
        var existingCategories = await categoryRepository.GetAllAsync();
        if (existingCategories.Any()) return; // Já tem dados, skip

        // Criar 5 Categorias de Tecnologia
        var notebooks = new Category { Name = "Notebooks" };
        var smartphones = new Category { Name = "Smartphones" };
        var perifericos = new Category { Name = "Periféricos" };
        var componentes = new Category { Name = "Componentes" };
        var acessorios = new Category { Name = "Acessórios" };

        await categoryRepository.AddAsync(notebooks);
        await categoryRepository.AddAsync(smartphones);
        await categoryRepository.AddAsync(perifericos);
        await categoryRepository.AddAsync(componentes);
        await categoryRepository.AddAsync(acessorios);
        
        await unitOfWork.CommitAsync();

        // Criar 20 Produtos realistas
        var products = new List<Product>
        {
            // Notebooks (4 produtos)
            new() { Name = "Dell XPS 13", Price = 8500.00m, CategoryId = notebooks.Id },
            new() { Name = "MacBook Air M2", Price = 12000.00m, CategoryId = notebooks.Id },
            new() { Name = "Lenovo ThinkPad X1", Price = 9500.00m, CategoryId = notebooks.Id },
            new() { Name = "Asus ROG Strix G16", Price = 10500.00m, CategoryId = notebooks.Id },

            // Smartphones (4 produtos)
            new() { Name = "iPhone 15 Pro", Price = 9000.00m, CategoryId = smartphones.Id },
            new() { Name = "Samsung Galaxy S24 Ultra", Price = 6500.00m, CategoryId = smartphones.Id },
            new() { Name = "Xiaomi 14 Pro", Price = 5200.00m, CategoryId = smartphones.Id },
            new() { Name = "Google Pixel 8 Pro", Price = 5800.00m, CategoryId = smartphones.Id },

            // Periféricos (4 produtos)
            new() { Name = "Mouse Logitech MX Master 3S", Price = 650.00m, CategoryId = perifericos.Id },
            new() { Name = "Teclado Mecânico Keychron K2", Price = 850.00m, CategoryId = perifericos.Id },
            new() { Name = "Monitor LG UltraWide 34\"", Price = 3200.00m, CategoryId = perifericos.Id },
            new() { Name = "Webcam Logitech C920", Price = 450.00m, CategoryId = perifericos.Id },

            // Componentes (4 produtos)
            new() { Name = "SSD Samsung 980 Pro 1TB", Price = 850.00m, CategoryId = componentes.Id },
            new() { Name = "Placa de Vídeo RTX 4070", Price = 4500.00m, CategoryId = componentes.Id },
            new() { Name = "Memória RAM Corsair 32GB DDR5", Price = 1200.00m, CategoryId = componentes.Id },
            new() { Name = "Processador Intel Core i9-13900K", Price = 3800.00m, CategoryId = componentes.Id },

            // Acessórios (4 produtos)
            new() { Name = "Headset Sony WH-1000XM5", Price = 2200.00m, CategoryId = acessorios.Id },
            new() { Name = "Hub USB-C Anker 7 em 1", Price = 280.00m, CategoryId = acessorios.Id },
            new() { Name = "Suporte para Notebook Ajustável", Price = 150.00m, CategoryId = acessorios.Id },
            new() { Name = "Mouse Pad Razer Gigantus V2", Price = 180.00m, CategoryId = acessorios.Id }
        };

        foreach (var product in products)
        {
            await productRepository.AddAsync(product);
        }
        
        await unitOfWork.CommitAsync();
    }
}
