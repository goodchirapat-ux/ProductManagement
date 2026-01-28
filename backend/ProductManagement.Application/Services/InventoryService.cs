namespace ProductManagement.Application.Services;

using ProductManagement.Application.DTOs;
using ProductManagement.Application.Helpers;
using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _repository;

    public InventoryService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<SellProductResponseDto> SellProductAsync(SellProductDto dto)
    {
        // Validate quantity
        if (dto.Quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero");

        if (dto.Quantity > int.MaxValue)
            throw new ArgumentException("Quantity exceeds allowed value");

        // Get product
        var product = await _repository.GetByIdAsync(dto.ProductId);
        if (product is null)
            throw new ProductNotFoundException(dto.ProductId);

        // Check stock
        if (product.Stock < dto.Quantity)
            throw new InsufficientStockException(product.Stock, dto.Quantity);

        // Update stock
        var newStock = product.Stock - dto.Quantity;
        var updatedProduct = product with { Stock = newStock, ModifiedWhen = DateTime.UtcNow };
        await _repository.UpdateAsync(updatedProduct);

        var responseDto = new ProductResponseDto(
            updatedProduct.Id,
            updatedProduct.Name,
            updatedProduct.Sku,
            updatedProduct.Price,
            updatedProduct.Stock,
            CategoryConverter.CategoryToString(updatedProduct.Category),
            updatedProduct.CreatedWhen,
            updatedProduct.ModifiedWhen
        );

        return new SellProductResponseDto(
            "Sell successful",
            newStock,
            responseDto
        );
    }
}
