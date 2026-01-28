namespace ProductManagement.Api.Tests.Controllers;

using FluentAssertions;
using Moq;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Entities;
using ProductManagement.Core.Exceptions;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ProductManagement.Api.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IProductService> _productServiceMock;
    private readonly Mock<IInventoryService> _inventoryServiceMock;
    private readonly Mock<IPricingService> _pricingServiceMock;
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _inventoryServiceMock = new Mock<IInventoryService>();
        _pricingServiceMock = new Mock<IPricingService>();
        _controller = new ProductsController(_productServiceMock.Object, _inventoryServiceMock.Object, _pricingServiceMock.Object);
    }

    #region GetProducts Tests

    [Fact]
    public async Task GetProducts_WithoutCategory_ReturnsAllProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<ProductResponseDto>
        {
            new(1, "Product 1", "SKU001", 10m, 5, "Food", now, now),
            new(2, "Product 2", "SKU002", 20m, 10, "Drink", now, now)
        };

        _productServiceMock.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts(null);

        // Assert
        result.Should().NotBeNull();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        var returnedProducts = Assert.IsType<List<ProductResponseDto>>(okResult.Value);
        returnedProducts.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProducts_WithCategory_ReturnsFilteredProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<ProductResponseDto>
        {
            new(1, "Food 1", "SKU001", 10m, 5, "Food", now, now),
            new(2, "Food 2", "SKU002", 20m, 10, "Food", now, now)
        };

        _productServiceMock.Setup(s => s.GetProductsByCategoryAsync(ProductCategory.Food)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetProducts("Food");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<ProductResponseDto>>(okResult.Value);
        returnedProducts.Should().AllSatisfy(p => p.Category.Should().Be("Food"));
    }

    #endregion

    #region GetProduct Tests

    [Fact]
    public async Task GetProduct_WithValidId_ReturnsProduct()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var product = new ProductResponseDto(1, "Product", "SKU123", 99.99m, 10, "Food", now, now);

        _productServiceMock.Setup(s => s.GetProductByIdAsync(1)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        var returnedProduct = Assert.IsType<ProductResponseDto>(okResult.Value);
        returnedProduct.Id.Should().Be(1);
    }

    [Fact]
    public async Task GetProduct_WithInvalidId_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetProduct(-1);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task GetProduct_WithNonExistentId_ThrowsNotFoundException()
    {
        // Arrange
        _productServiceMock.Setup(s => s.GetProductByIdAsync(999))
            .ThrowsAsync(new ProductNotFoundException(999));

        // Act
        Func<Task> act = () => _controller.GetProduct(999);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    #endregion

    #region SearchProducts Tests

    [Fact]
    public async Task SearchProducts_WithKeyword_ReturnsMatchingProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<ProductResponseDto>
        {
            new(1, "Test Product", "TEST123", 10m, 5, "Food", now, now)
        };

        _productServiceMock.Setup(s => s.SearchProductsAsync("Test")).ReturnsAsync(products);

        // Act
        var result = await _controller.SearchProducts("Test");

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProducts = Assert.IsType<List<ProductResponseDto>>(okResult.Value);
        returnedProducts.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchProducts_WithNullKeyword_ReturnsAllProducts()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var products = new List<ProductResponseDto>
        {
            new(1, "Product 1", "SKU001", 10m, 5, "Food", now, now)
        };

        _productServiceMock.Setup(s => s.SearchProductsAsync("")).ReturnsAsync(products);

        // Act
        var result = await _controller.SearchProducts(null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.Should().NotBeNull();
    }

    #endregion

    #region CreateProduct Tests

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateProductDto("New Product", "NEWSKU", 99.99m, 10, "Food");
        var now = DateTime.UtcNow;
        var createdProduct = new ProductResponseDto(1, "New Product", "NEWSKU", 99.99m, 10, "Food", now, now);

        _productServiceMock.Setup(s => s.CreateProductAsync(createDto)).ReturnsAsync(createdProduct);

        // Act
        var result = await _controller.CreateProduct(createDto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        createdResult.StatusCode.Should().Be(StatusCodes.Status201Created);
        createdResult.ActionName.Should().Be(nameof(_controller.GetProduct));
        var returnedProduct = Assert.IsType<ProductResponseDto>(createdResult.Value);
        returnedProduct.Id.Should().Be(1);
    }

    [Fact]
    public async Task CreateProduct_WithInvalidData_ThrowsException()
    {
        // Arrange
        var createDto = new CreateProductDto("", "SKU123", 99.99m, 10, "Food"); // Empty name
        _productServiceMock.Setup(s => s.CreateProductAsync(createDto))
            .ThrowsAsync(new ArgumentException("Validation failed"));

        // Act
        Func<Task> act = () => _controller.CreateProduct(createDto);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    #endregion

    #region SellProduct Tests

    [Fact]
    public async Task SellProduct_WithValidData_ReturnsOk()
    {
        // Arrange
        var sellDto = new SellProductDto(1, 5);
        var now = DateTime.UtcNow;
        var product = new ProductResponseDto(1, "Product", "SKU123", 99.99m, 5, "Food", now, now);
        var response = new SellProductResponseDto("Sell successful", 5, product);

        _inventoryServiceMock.Setup(s => s.SellProductAsync(sellDto)).ReturnsAsync(response);

        // Act
        var result = await _controller.SellProduct(sellDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        var returnedResponse = Assert.IsType<SellProductResponseDto>(okResult.Value);
        returnedResponse.Message.Should().Contain("successful");
    }

    [Fact]
    public async Task SellProduct_WithInsufficientStock_ThrowsException()
    {
        // Arrange
        var sellDto = new SellProductDto(1, 100);
        _inventoryServiceMock.Setup(s => s.SellProductAsync(sellDto))
            .ThrowsAsync(new InsufficientStockException(10, 100));

        // Act
        Func<Task> act = () => _controller.SellProduct(sellDto);

        // Assert
        await act.Should().ThrowAsync<InsufficientStockException>();
    }

    [Fact]
    public async Task SellProduct_WithNonExistentProduct_ThrowsNotFoundException()
    {
        // Arrange
        var sellDto = new SellProductDto(999, 5);
        _inventoryServiceMock.Setup(s => s.SellProductAsync(sellDto))
            .ThrowsAsync(new ProductNotFoundException(999));

        // Act
        Func<Task> act = () => _controller.SellProduct(sellDto);

        // Assert
        await act.Should().ThrowAsync<ProductNotFoundException>();
    }

    #endregion

    #region BulkPriceUpdate Tests

    [Fact]
    public async Task BulkPriceUpdate_WithValidData_ReturnsOk()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 49.99m),
            new(2, 29.99m)
        });
        var response = new BulkPriceUpdateResponseDto("Bulk update completed", 2, 0, 2);

        _pricingServiceMock.Setup(s => s.BulkUpdatePricesAsync(updates)).ReturnsAsync(response);

        // Act
        var result = await _controller.BulkPriceUpdate(updates);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        var returnedResponse = Assert.IsType<BulkPriceUpdateResponseDto>(okResult.Value);
        returnedResponse.UpdatedItems.Should().Be(2);
    }

    [Fact]
    public async Task BulkPriceUpdate_WithEmptyList_ReturnsBadRequest()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>());

        // Act
        var result = await _controller.BulkPriceUpdate(updates);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        badRequestResult.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task BulkPriceUpdate_WithDuplicateIds_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, 49.99m),
            new(1, 59.99m)
        });

        _pricingServiceMock.Setup(s => s.BulkUpdatePricesAsync(updates))
            .ThrowsAsync(new ArgumentException("Duplicate product IDs"));

        // Act
        Func<Task> act = () => _controller.BulkPriceUpdate(updates);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task BulkPriceUpdate_WithInvalidPrices_ThrowsException()
    {
        // Arrange
        var updates = new BulkPriceUpdateDto(new List<PriceUpdateItem>
        {
            new(1, -5m)
        });

        _pricingServiceMock.Setup(s => s.BulkUpdatePricesAsync(updates))
            .ThrowsAsync(new InvalidPriceException(-5m));

        // Act
        Func<Task> act = () => _controller.BulkPriceUpdate(updates);

        // Assert
        await act.Should().ThrowAsync<InvalidPriceException>();
    }

    #endregion
}
