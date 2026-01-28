namespace ProductManagement.Core.Validators;

using FluentValidation;
using ProductManagement.Core.Entities;

public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(IEnumerable<Product> existingProducts)
    {
        var existingList = existingProducts.ToList();

        RuleFor(p => p.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.")
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .WithMessage("Name cannot be only whitespace.");

        RuleFor(p => p.Sku)
            .NotEmpty().WithMessage("SKU is required.")
            .MinimumLength(3).WithMessage("SKU must be at least 3 characters.")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.")
            .Must(sku => !existingList.Any(p => p.Sku == sku))  
            .WithMessage("SKU must be unique.");

        RuleFor(p => p.Price)
            .NotEmpty().WithMessage("Price is required.")
            .GreaterThan(0).WithMessage("Price must be greater than 0.")
            .LessThanOrEqualTo(999999.99m).WithMessage("Price cannot exceed 999,999.99.")
            .Must(p => p % 0.01m == 0).WithMessage("Price can only have up to 2 decimal places.");

        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.")
            .LessThanOrEqualTo(int.MaxValue).WithMessage("Stock exceeds maximum allowed value.");

        RuleFor(p => p.Category)
            .IsInEnum().WithMessage("Invalid category selected.");
    }
}
