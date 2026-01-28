namespace ProductManagement.Application.Services;

using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Core.Exceptions;

public class PricingService : IPricingService
{
    private readonly IProductRepository _repository;

    public PricingService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<BulkPriceUpdateResponseDto> BulkUpdatePricesAsync(BulkPriceUpdateDto dto)
    {
        // Validate inputs
        if (dto.Updates is null || !dto.Updates.Any())
            return new BulkPriceUpdateResponseDto("No updates provided", 0, 0, 0);

        // Check for duplicates
        var duplicateIds = dto.Updates
            .GroupBy(u => u.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateIds.Any())
            throw new ArgumentException($"Duplicate product IDs in request: {string.Join(", ", duplicateIds)}");

        // Validate all prices before making updates
        var invalidPrices = dto.Updates.Where(u => u.NewPrice <= 0 || u.NewPrice > 999999.99m || u.NewPrice % 0.01m != 0).ToList();
        if (invalidPrices.Any())
            throw new InvalidPriceException(invalidPrices.First().NewPrice);

        int updatedCount = 0;
        int failedCount = 0;

        foreach (var update in dto.Updates)
        {
            try
            {
                var product = await _repository.GetByIdAsync(update.ProductId);
                if (product is null)
                {
                    failedCount++;
                    continue;
                }

                var updatedProduct = product with { Price = update.NewPrice, ModifiedWhen = DateTime.UtcNow };
                await _repository.UpdateAsync(updatedProduct);
                updatedCount++;
            }
            catch
            {
                failedCount++;
            }
        }

        return new BulkPriceUpdateResponseDto(
            "Bulk update completed",
            updatedCount,
            failedCount,
            dto.Updates.Count
        );
    }
}
