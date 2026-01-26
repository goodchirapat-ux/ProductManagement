namespace entity;

using FluentValidation;

public record Product(int Id, string Name, string Sku, decimal Price, int Stock, ProductCategory Category);

public enum ProductCategory
{
    Food,
    Drink,
    Appliance,
    Clothes
}

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(List<Product> existingProducts)
    {
        RuleFor(p => p.Name).NotEmpty().WithMessage("Name is required.");
        
        RuleFor(p => p.Sku)
            .NotEmpty()
            .MinimumLength(3)
            .Must((product, sku) => !existingProducts.Any(p => p.Sku == sku && p.Id != product.Id))
            .WithMessage("SKU must be unique and at least 3 characters.");

        RuleFor(p => p.Price).NotEmpty().GreaterThan(0).WithMessage("Price must be more than 0.");
        
        RuleFor(p => p.Stock).NotEmpty().GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        
        RuleFor(p => p.Category).IsInEnum().WithMessage("Invalid category selected.");
    }
}

