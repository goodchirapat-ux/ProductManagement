namespace ProductManagement.Infrastructure.Repositories;

using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Entities;

public class ProductRepository : IProductRepository
{
    private readonly InMemoryProductStore _store;

    public ProductRepository(InMemoryProductStore store)
    {
        _store = store;
    }

    public Task<Product?> GetByIdAsync(int id)
    {
        var product = _store.GetProductById(id);
        return Task.FromResult(product);
    }

    public Task<IEnumerable<Product>> GetAllAsync()
    {
        var products = _store.GetAllProducts().AsEnumerable();
        return Task.FromResult(products);
    }

    public Task<IEnumerable<Product>> GetByCategoryAsync(ProductCategory category)
    {
        var products = _store.GetProductsByCategory(category);
        return Task.FromResult(products);
    }

    public Task<Product> AddAsync(Product product)
    {
        var newProduct = _store.AddProduct(product);
        return Task.FromResult(newProduct);
    }

    public Task<Product> UpdateAsync(Product product)
    {
        _store.UpdateProduct(product);
        return Task.FromResult(product);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var deleted = _store.DeleteProduct(id);
        return Task.FromResult(deleted);
    }

    public Task<bool> SkuExistsAsync(string sku, int excludeId = 0)
    {
        var exists = _store.SkuExists(sku, excludeId);
        return Task.FromResult(exists);
    }

    public Task<IEnumerable<Product>> SearchAsync(string keyword)
    {
        var results = _store.SearchProducts(keyword);
        return Task.FromResult(results);
    }
}
