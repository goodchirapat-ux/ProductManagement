namespace ProductManagement.Application.Services;

using ProductManagement.Application.DTOs;
using ProductManagement.Application.Helpers;
using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;
using ProductManagement.Core.Validators;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto)
    {
        var existingProducts = await _repository.GetAllAsync();
        var validator = new ProductValidator(existingProducts);

        var now = DateTime.UtcNow;
        var category = CategoryConverter.StringToCategory(dto.Category);
        var product = new Product(
            Id: 0,
            Name: dto.Name,
            Sku: dto.Sku,
            Price: dto.Price,
            Stock: dto.Stock,
            Category: category,
            CreatedWhen: now,
            ModifiedWhen: now
        );

        var validationResult = validator.Validate(product);
        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            throw new ArgumentException($"Validation failed: {errors}");
        }

        var createdProduct = await _repository.AddAsync(product);
        return MapToResponseDto(createdProduct);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync()
    {
        var products = await _repository.GetAllAsync();
        return products.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(ProductCategory category)
    {
        var products = await _repository.GetByCategoryAsync(category);
        return products.Select(MapToResponseDto);
    }

    public async Task<ProductResponseDto> GetProductByIdAsync(int id)
    {
        if (id <= 0)
            throw new ArgumentException("Product ID must be a positive integer");

        var product = await _repository.GetByIdAsync(id);
        if (product is null)
            throw new ProductNotFoundException(id);

        return MapToResponseDto(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await GetAllProductsAsync();
        }

        var products = await _repository.SearchAsync(keyword);
        return products.Select(MapToResponseDto);
    }

    private static ProductResponseDto MapToResponseDto(Product product)
    {
        return new ProductResponseDto(
            product.Id,
            product.Name,
            product.Sku,
            product.Price,
            product.Stock,
            CategoryConverter.CategoryToString(product.Category),
            product.CreatedWhen,
            product.ModifiedWhen
        );
    }
}
