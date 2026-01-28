namespace ProductManagement.Application.Tests.Services;

using FluentAssertions;
using Moq;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;
using Xunit;

public class InventoryServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new InventoryService(_repositoryMock.Object);
    }

    [Fact]
    public async Task SellProductAsync_WithValidQuantity_ReducesStock()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU123", 99.99m, 10, ProductCategory.Food, now, now);
        var sellDto = new SellProductDto(1, 3);

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product with { Stock = 7 });

        // Act
        var result = await _service.SellProductAsync(sellDto);

        // Assert
        result.Should().NotBeNull();
        result.RemainingStock.Should().Be(7);
        result.Message.Should().Contain("successful");
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task SellProductAsync_WithZeroQuantity_ThrowsException()
    {
        // Arrange
        var sellDto = new SellProductDto(1, 0);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SellProductAsync(sellDto));
    }

    [Fact]
    public async Task SellProductAsync_WithNegativeQuantity_ThrowsException()
    {
        // Arrange
        var sellDto = new SellProductDto(1, -5);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.SellProductAsync(sellDto));
    }

    [Fact]
    public async Task SellProductAsync_WithNonExistentProduct_ThrowsNotFoundException()
    {
        // Arrange
        var sellDto = new SellProductDto(999, 5);

        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ProductNotFoundException>(() => _service.SellProductAsync(sellDto));
    }

    [Fact]
    public async Task SellProductAsync_WithInsufficientStock_ThrowsException()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU123", 99.99m, 5, ProductCategory.Food, now, now);
        var sellDto = new SellProductDto(1, 10); // Trying to sell more than available

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act & Assert
        await Assert.ThrowsAsync<InsufficientStockException>(() => _service.SellProductAsync(sellDto));
    }

    [Fact]
    public async Task SellProductAsync_WithExactStock_ReducesToZero()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU123", 99.99m, 5, ProductCategory.Food, now, now);
        var sellDto = new SellProductDto(1, 5); // Sell entire stock

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync(product with { Stock = 0 });

        // Act
        var result = await _service.SellProductAsync(sellDto);

        // Assert
        result.RemainingStock.Should().Be(0);
    }

    [Fact]
    public async Task SellProductAsync_WithExcessiveQuantity_ThrowsException()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU123", 99.99m, 10, ProductCategory.Food, now, now);
        var sellDto = new SellProductDto(1, int.MaxValue);

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

        // Act & Assert
        // int.MaxValue is not > int.MaxValue, so it passes the ArgumentException check
        // But stock (10) < quantity (int.MaxValue), so throws InsufficientStockException
        await Assert.ThrowsAsync<InsufficientStockException>(() => _service.SellProductAsync(sellDto));
    }
}
