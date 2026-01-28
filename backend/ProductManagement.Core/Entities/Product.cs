namespace ProductManagement.Core.Entities;

public record Product(
    int Id,
    string Name,
    string Sku,
    decimal Price,
    int Stock,
    ProductCategory Category,
    DateTime CreatedWhen,
    DateTime ModifiedWhen
);

public enum ProductCategory
{
    Food,
    Drink,
    Appliance,
    Clothes
}
