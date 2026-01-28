namespace ProductManagement.Application.DTOs;

using ProductManagement.Core.Entities;

// Input DTOs - Accept category as string
public record CreateProductDto(
    string Name,
    string Sku,
    decimal Price,
    int Stock,
    string Category
);

// Output DTOs - Return category as string
public record ProductResponseDto(
    int Id,
    string Name,
    string Sku,
    decimal Price,
    int Stock,
    string Category,
    DateTime CreatedWhen,
    DateTime ModifiedWhen
);

public record SellProductDto(
    int ProductId,
    int Quantity
);

public record SellProductResponseDto(
    string Message,
    int RemainingStock,
    ProductResponseDto Product
);

public record BulkPriceUpdateDto(
    List<PriceUpdateItem> Updates
);

public record BulkPriceUpdateResponseDto(
    string Message,
    int UpdatedItems,
    int FailedItems,
    int TotalRequested
);

public record SearchProductsDto(
    string? Keyword
);
