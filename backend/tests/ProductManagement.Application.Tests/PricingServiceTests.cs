namespace ProductManagement.Application.Tests.Services;

using FluentAssertions;
using Moq;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;
using Xunit;

public class PricingServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly PricingService _service;

    public PricingServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _service = new PricingService(_repositoryMock.Object);
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithValidPrices_UpdatesSuccessfully()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product1 = new Product(1, "Product 1", "SKU001", 10m, 5, ProductCategory.Food, now, now);
        var product2 = new Product(2, "Product 2", "SKU002", 20m, 10, ProductCategory.Drink, now, now);

        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 15m),
            new(2, 25m)
        });

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _repositoryMock.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(product2);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

        // Act
        var result = await _service.BulkUpdatePricesAsync(updates);

        // Assert
        result.UpdatedItems.Should().Be(2);
        result.FailedItems.Should().Be(0);
        result.TotalRequested.Should().Be(2);
        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Exactly(2));
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithEmptyList_ReturnsZeroUpdates()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>());

        // Act
        var result = await _service.BulkUpdatePricesAsync(updates);

        // Assert
        result.UpdatedItems.Should().Be(0);
        result.FailedItems.Should().Be(0);
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithNullList_ReturnsZeroUpdates()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(null!);

        // Act
        var result = await _service.BulkUpdatePricesAsync(updates);

        // Assert
        result.UpdatedItems.Should().Be(0);
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithDuplicateIds_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 15m),
            new(1, 25m) // Duplicate product ID
        });

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _service.BulkUpdatePricesAsync(updates));
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithInvalidPrice_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, -5m) // Negative price
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPriceException>(() => _service.BulkUpdatePricesAsync(updates));
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithExcessivePrice_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 1000000m) // Exceeds max
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPriceException>(() => _service.BulkUpdatePricesAsync(updates));
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithZeroPrice_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 0m)
        });

        // Act & Assert
        await Assert.ThrowsAsync<InvalidPriceException>(() => _service.BulkUpdatePricesAsync(updates));
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithMixedResults_ReturnsSummary()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product1 = new Product(1, "Product 1", "SKU001", 10m, 5, ProductCategory.Food, now, now);

        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 15m),
            new(999, 25m) // Product doesn't exist
        });

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product1);
        _repositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Product?)null);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

        // Act
        var result = await _service.BulkUpdatePricesAsync(updates);

        // Assert
        result.UpdatedItems.Should().Be(1);
        result.FailedItems.Should().Be(1);
        result.TotalRequested.Should().Be(2);
    }

    [Fact]
    public async Task BulkUpdatePricesAsync_WithValidDecimalPrice_UpdatesSuccessfully()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new Product(1, "Product", "SKU001", 10m, 5, ProductCategory.Food, now, now);

        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 15.99m) // Valid with 2 decimals
        });

        _repositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
        _repositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>())).ReturnsAsync((Product p) => p);

        // Act
        var result = await _service.BulkUpdatePricesAsync(updates);

        // Assert
        result.UpdatedItems.Should().Be(1);
    }
}
