namespace ProductManagement.Core.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public class ProductNotFoundException : DomainException
{
    public ProductNotFoundException(int productId)
        : base($"Product with ID {productId} not found") { }
}

public class InsufficientStockException : DomainException
{
    public InsufficientStockException(int available, int requested)
        : base($"Insufficient stock. Available: {available}, Requested: {requested}") { }
}

public class DuplicateSkuException : DomainException
{
    public DuplicateSkuException(string sku)
        : base($"A product with SKU '{sku}' already exists") { }
}

public class InvalidPriceException : DomainException
{
    public InvalidPriceException(decimal price)
        : base($"Invalid price: {price}. Price must be greater than 0 and have at most 2 decimal places.") { }
}
