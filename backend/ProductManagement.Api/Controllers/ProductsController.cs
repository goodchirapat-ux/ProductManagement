namespace ProductManagement.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Helpers;
using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Entities;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IInventoryService _inventoryService;
    private readonly IPricingService _pricingService;

    public ProductsController(
        IProductService productService,
        IInventoryService inventoryService,
        IPricingService pricingService)
    {
        _productService = productService;
        _inventoryService = inventoryService;
        _pricingService = pricingService;
    }

    /// <summary>
    /// Get all products, optionally filtered by category
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts([FromQuery] string? category)
    {
        if (!string.IsNullOrWhiteSpace(category))
        {
            var categoryEnum = CategoryConverter.StringToCategory(category);
            var products = await _productService.GetProductsByCategoryAsync(categoryEnum);
            return Ok(products);
        }

        var allProducts = await _productService.GetAllProductsAsync();
        return Ok(allProducts);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductResponseDto>> GetProduct(int id)
    {
        if (id <= 0)
            return BadRequest(new { error = "Product ID must be a positive integer" });

        var product = await _productService.GetProductByIdAsync(id);
        return Ok(product);
    }

    /// <summary>
    /// Search products by keyword (name or SKU)
    /// </summary>
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<ProductResponseDto>>> SearchProducts([FromQuery] string? keyword)
    {
        var products = await _productService.SearchProductsAsync(keyword ?? "");
        return Ok(products);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var product = await _productService.CreateProductAsync(dto);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    /// <summary>
    /// Sell a product (reduce stock)
    /// </summary>
    [HttpPost("sell")]
    public async Task<ActionResult<SellProductResponseDto>> SellProduct([FromBody] SellProductDto dto)
    {
        var result = await _inventoryService.SellProductAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Bulk update prices for multiple products
    /// </summary>
    [HttpPut("bulk-price-update")]
    public async Task<ActionResult<BulkPriceUpdateResponseDto>> BulkPriceUpdate([FromBody] BulkPriceUpdateDto dto)
    {
        if (dto.Updates is null || !dto.Updates.Any())
            return BadRequest(new { error = "Updates list cannot be empty" });

        var result = await _pricingService.BulkUpdatePricesAsync(dto);
        return Ok(result);
    }
}
