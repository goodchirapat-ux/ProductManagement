# ProductManagement Unit Tests

This folder contains comprehensive unit tests for the ProductManagement application.

## Project Structure

```
tests/
├── ProductManagement.Core.Tests/
│   └── ProductValidatorTests.cs
├── ProductManagement.Application.Tests/
│   ├── ProductServiceTests.cs
│   ├── InventoryServiceTests.cs
│   └── PricingServiceTests.cs
└── ProductManagement.Api.Tests/
    └── (Integration tests - coming soon)
```

## Test Coverage

### Core Layer Tests

- **ProductValidatorTests** - 11 test cases
  - Valid product validation
  - Empty name validation
  - Duplicate SKU detection
  - Price validation (negative, zero, excessive)
  - Stock validation (negative values)
  - SKU format validation
  - Name length validation

### Application Layer Tests

#### ProductServiceTests - 8 test cases

- Create product with valid data
- Create product with invalid data
- Get all products
- Get product by ID (valid & invalid)
- Get products by category
- Search products with keyword
- Search products with empty keyword

#### InventoryServiceTests - 7 test cases

- Sell product with valid quantity
- Sell with zero quantity
- Sell with negative quantity
- Sell non-existent product
- Sell with insufficient stock
- Sell exact stock (reduce to zero)
- Sell with excessive quantity

#### PricingServiceTests - 8 test cases

- Bulk update valid prices
- Bulk update with empty list
- Bulk update with null list
- Bulk update with duplicate IDs
- Bulk update with invalid prices
- Bulk update with excessive prices
- Bulk update with zero price
- Bulk update with mixed results

## Running Tests

### Run All Tests

```bash
cd c:\FlowAccount\backend
dotnet test
```

### Run Specific Test Project

```bash
dotnet test tests/ProductManagement.Core.Tests
dotnet test tests/ProductManagement.Application.Tests
```

### Run with Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
```

### Run Tests in Verbose Mode

```bash
dotnet test --verbosity detailed
```

### Run Single Test Class

```bash
dotnet test --filter "ClassName=ProductValidatorTests"
```

### Run Single Test Method

```bash
dotnet test --filter "MethodName=Validate_WithValidProduct_ReturnsSuccess"
```

## Testing Frameworks & Tools

- **xUnit** - Test framework (industry standard for .NET)
- **Moq** - Mocking library for dependencies
- **FluentAssertions** - Readable assertion library

## Test Patterns Used

### Arrange-Act-Assert (AAA)

```csharp
// Arrange - Set up test data
var createDto = new CreateProductDto(...);

// Act - Execute the action
var result = await _service.CreateProductAsync(createDto);

// Assert - Verify the result
result.Should().NotBeNull();
```

### Mocking Dependencies

```csharp
var _repositoryMock = new Mock<IProductRepository>();
_repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);
```
