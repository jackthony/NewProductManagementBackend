using FluentValidation;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.DTOs.Response;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Repositories;

namespace ProductManagement.Application.Services;

public class ProductService
{
    private readonly ProductRepository _repository;
    private readonly IValidator<ProductRequestDto> _validator;

    public ProductService(ProductRepository repository, IValidator<ProductRequestDto> validator)
    {
        _repository = repository;
        _validator = validator;
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(p => new ProductResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Stock = p.Stock,
            CategoryId = p.CategoryId
        });
    }

    public async Task<ProductResponseDto?> GetByIdAsync(int id)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
    }
    public async Task<ProductResponseDto> AddAsync(ProductRequestDto productRequestDto)
    {
        // Validar el DTO antes de continuar
        var validationResults = await _validator.ValidateAsync(productRequestDto);
        if (!validationResults.IsValid)
            throw new FluentValidation.ValidationException(validationResults.Errors);

        // Verificar si la categoría existe
        var categoryExists = await _repository.CategoryExists(productRequestDto.CategoryId);
        if (!categoryExists)
            throw new KeyNotFoundException($"La categoría con ID {productRequestDto.CategoryId} no existe.");

        var product = new Product
        {
            Name = productRequestDto.Name,
            Description = productRequestDto.Description,
            Price = productRequestDto.Price,
            Stock = productRequestDto.Stock,
            CategoryId = productRequestDto.CategoryId
        };

        await _repository.AddAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
    }


    public async Task<ProductResponseDto> UpdateAsync(int id, ProductRequestDto productRequestDto)
    {
        var product = await _repository.GetByIdAsync(id);
        if (product == null) throw new KeyNotFoundException($"Producto con ID {id} no encontrado.");

        product.Name = productRequestDto.Name;
        product.Description = productRequestDto.Description;
        product.Price = productRequestDto.Price;
        product.Stock = productRequestDto.Stock;
        product.CategoryId = productRequestDto.CategoryId;

        await _repository.UpdateAsync(product);

        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Stock = product.Stock,
            CategoryId = product.CategoryId
        };
    }


    public async Task DeleteAsync(int id)
    {
        await _repository.DeleteAsync(id);
    }
}
