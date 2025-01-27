using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Services;
using ProductManagement.Application.Validators;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Infrastructure.Repositories;
using Xunit;
public class ProductServiceTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly ProductRepository _repository;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        // Configuración del contexto en memoria
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Base de datos única por prueba
            .Options;

        _dbContext = new AppDbContext(options, useSeedData: false);
        _repository = new ProductRepository(_dbContext);

        // Usar un validador real
        var validator = new ProductValidator();

        // Inicializar el servicio con el repositorio y el validador
        _service = new ProductService(_repository, new ProductValidator());

        SeedData(); // Insertar datos iniciales para cada prueba
    }

    private void SeedData()
    {
        var category = new Category { Name = "Electronics" };
        _dbContext.Categories.Add(category);

        _dbContext.Products.Add(new Product
        {
            Name = "Laptop",
            Description = "High-end laptop",
            Price = 1500,
            Stock = 10,
            Category = category
        });

        _dbContext.SaveChanges();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted(); // Elimina todos los datos al finalizar la prueba
        _dbContext.Dispose();
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnProducts()
    {
        // Act
        var products = await _service.GetAllAsync();

        // Assert
        Assert.NotNull(products);
        Assert.Single(products);
        Assert.Equal("Laptop", products.First().Name);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // Arrange
        var productDto = new ProductRequestDto
        {
            Name = "Smartphone",
            Description = "Latest model",
            Price = 1000,
            Stock = 20,
            CategoryId = 1
        };

        // Act
        await _service.AddAsync(productDto);

        // Assert
        var products = await _service.GetAllAsync();
        Assert.Equal(2, products.Count()); // Verifica que ahora hay dos productos
        Assert.Equal("Smartphone", products.Last().Name);
    }

    //Probar qué sucede si intentas agregar un producto con una categoría inexistente:
    [Fact]
    public async Task AddAsync_ShouldThrowException_WhenCategoryDoesNotExist()
    {
        // Arrange
        var productDto = new ProductRequestDto
        {
            Name = "Invalid Product",
            Description = "Invalid Category",
            Price = 100,
            Stock = 10,
            CategoryId = 999 // Categoría inexistente
        };

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.AddAsync(productDto));
    }

    //Probar qué sucede si intentas obtener un producto con un Id inexistente
    [Fact]
    public async Task GetByIdAsync_ShouldThrowException_WhenProductDoesNotExist()
    {
        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(999));
    }


    //Probar la actualización de un producto:
    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // Arrange
        var productDto = new ProductRequestDto
        {
            Name = "Updated Product",
            Description = "Updated Description",
            Price = 2000,
            Stock = 15,
            CategoryId = 1
        };

        // Act
        await _service.UpdateAsync(1, productDto);

        // Assert
        var updatedProduct = await _service.GetByIdAsync(1);
        Assert.Equal("Updated Product", updatedProduct?.Name);
        Assert.Equal(2000, updatedProduct?.Price);
    }


    //Probar la eliminación de un producto:
    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct()
    {
        // Act
        await _service.DeleteAsync(1);

        // Assert
        var products = await _service.GetAllAsync();
        Assert.Empty(products);
    }


    //Probar que no se puede agregar un producto con un precio negativo

    [Fact]
    public async Task AddAsync_ShouldThrowValidationException_WhenPriceIsNegative()
    {
        // Arrange
        var productDto = new ProductRequestDto
        {
            Name = "Invalid Product",
            Description = "Negative Price",
            Price = -100, // Precio negativo
            Stock = 10,
            CategoryId = 1
        };

        // Act & Assert
        await Assert.ThrowsAsync<FluentValidation.ValidationException>(() => _service.AddAsync(productDto));
    }



}
