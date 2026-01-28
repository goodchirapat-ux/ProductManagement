namespace ProductManagement.Infrastructure.Repositories;

using ProductManagement.Core.Entities;

/// <summary>
/// Singleton in-memory data store for products.
/// This class maintains the shared state across all repository instances.
/// </summary>
public class InMemoryProductStore
{
    private readonly List<Product> _products = new();
    private int _nextId = 1;

    public List<Product> GetAllProducts()
    {
        return _products;
    }

    public Product? GetProductById(int id)
    {
        return _products.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<Product> GetProductsByCategory(ProductCategory category)
    {
        return _products.Where(p => p.Category == category);
    }

    public Product AddProduct(Product product)
    {
        var newProduct = product with { Id = _nextId++ };
        _products.Add(newProduct);
        return newProduct;
    }

    public void UpdateProduct(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index >= 0)
        {
            _products[index] = product;
        }
    }

    public bool DeleteProduct(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product is not null)
        {
            _products.Remove(product);
            return true;
        }
        return false;
    }

    public bool SkuExists(string sku, int excludeId = 0)
    {
        return _products.Any(p => p.Sku == sku && p.Id != excludeId);
    }

    public IEnumerable<Product> SearchProducts(string keyword)
    {
        return _products.Where(p =>
            p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            p.Sku.Contains(keyword, StringComparison.OrdinalIgnoreCase)
        );
    }
}
