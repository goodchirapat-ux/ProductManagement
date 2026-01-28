namespace ProductManagement.Application.Tests.Services;

using FluentAssertions;
using Moq;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;
using Xunit;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new ProductService(_repositoryMock.Object);
    }

    [Fact]
    public async Task CreateProductAsync_WithValidData_ReturnsSavedProduct()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var createDto = new CreateProductDto("Test Product", "SKU123", 99.99m, 10, "Food");
        var savedProduct = new Product(1, "Test Product", "SKU123", 99.99m, 10, ProductCategory.Food, now, now);

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>())).ReturnsAsync(savedProduct);

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Test Product");
        result.Sku.Should().Be("SKU123");
        result.Category.Should().Be("Food");
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task CreateProductAsync_WithInvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new CreateProductDto("", "SKU123", 99.99m, 10, "Food"); // Empty name

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateProductAsync(createDto));
    }

    [Fact]
    public async Task GetAllProductsAsync_ReturnsAllProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new(1, "Product 1", "SKU001", 10m, 5, ProductCategory.Food, now, now),
            new(2, "Product 2", "SKU002", 20m, 10, ProductCategory.Drink, now, now)
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.GetAllProductsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Should().NotBeNull());
    }

    [Fact]
    public async Task GetProductByIdAsync_WithValidId_ReturnsProduct()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU123", 99.99m, 10, ProductCategory.Food, now, now);

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _service.GetProductByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Product");
    }

    [Fact]
    public async Task GetProductByIdAsync_WithInvalidId_ThrowsNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => _service.GetProductByIdAsync(999));
    }

    [Fact]
    public async Task GetProductByIdAsync_WithNegativeId_ThrowsArgumentException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.GetProductByIdAsync(-1));
    }

    [Fact]
    public async Task GetProductsByCategoryAsync_ReturnsCategoryProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new(1, "Food 1", "SKU001", 10m, 5, ProductCategory.Food, now, now),
            new(2, "Food 2", "SKU002", 20m, 10, ProductCategory.Food, now, now)
        };

        _repositoryMock.Setup(r => r.GetByCategoryAsync(ProductCategory.Food)).ReturnsAsync(products);

        // Act
        var result = await _service.GetProductsByCategoryAsync(ProductCategory.Food);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(p => p.Category.Should().Be("Food"));
    }

    [Fact]
    public async Task SearchProductsAsync_WithKeyword_ReturnsMatchingProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new(1, "Test Product", "TEST123", 10m, 5, ProductCategory.Food, now, now)
        };

        _repositoryMock.Setup(r => r.SearchAsync("Test")).ReturnsAsync(products);

        // Act
        var result = await _service.SearchProductsAsync("Test");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Contain("Test");
    }

    [Fact]
    public async Task SearchProductsAsync_WithEmptyKeyword_ReturnsAllProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new(1, "Product 1", "SKU001", 10m, 5, ProductCategory.Food, now, now),
            new(2, "Product 2", "SKU002", 20m, 10, ProductCategory.Drink, now, now)
        };

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        // Act
        var result = await _service.SearchProductsAsync("");

        // Assert
        result.Should().HaveCount(2);
    }
}
