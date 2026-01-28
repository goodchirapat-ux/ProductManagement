namespace ProductManagement.Application.Interfaces;

using ProductManagement.Application.DTOs;
using ProductManagement.Core.Entities;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> SkuExistsAsync(string sku, int excludeId = 0);
    Task<IEnumerable<Product>> SearchAsync(string keyword);
}

public interface IProductService
{
    Task<ProductResponseDto> CreateProductAsync(CreateProductDto dto);
    Task<IEnumerable<ProductResponseDto>> GetAllProductsAsync();
    Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(ProductCategory category);
    Task<ProductResponseDto> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductResponseDto>> SearchProductsAsync(string keyword);
}

public interface IInventoryService
{
    Task<SellProductResponseDto> SellProductAsync(SellProductDto dto);
}

public interface IPricingService
{
    Task<BulkPriceUpdateResponseDto> BulkUpdatePricesAsync(BulkPriceUpdateDto dto);
}
