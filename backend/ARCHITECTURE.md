# ProductManagement Backend - Refactored Architecture

## Overview

The backend has been restructured following **Clean Architecture** principles with a clear separation of concerns across four projects.

## Project Structure

```
backend/
├── ProductManagement.sln (Main solution file)
├── ProductManagement.Core/                   (Domain layer)
│   ├── Entities/
│   │   ├── Product.cs
│   │   ├── SellRequest.cs
│   │   └── PriceUpdateItem.cs
│   ├── Exceptions/
│   │   └── DomainExceptions.cs
│   ├── Validators/
│   │   └── ProductValidator.cs
│   └── ProductManagement.Core.csproj
│
├── ProductManagement.Application/            (Business logic layer)
│   ├── Services/
│   │   ├── ProductService.cs
│   │   ├── InventoryService.cs
│   │   └── PricingService.cs
│   ├── DTOs/
│   │   └── ProductDtos.cs
│   ├── Interfaces/
│   │   └── IProductService.cs
│   └── ProductManagement.Application.csproj
│
├── ProductManagement.Infrastructure/         (Data access layer)
│   ├── Repositories/
│   │   └── ProductRepository.cs
│   └── ProductManagement.Infrastructure.csproj
│
└── ProductManagement.Api/                    (API/Presentation layer)
    ├── Controllers/
    │   └── ProductsController.cs
    ├── Middleware/
    │   └── ErrorHandlingMiddleware.cs
    ├── Properties/
    │   └── launchSettings.json
    ├── Program.cs
    ├── appsettings.json
    ├── appsettings.Development.json
    └── ProductManagement.Api.csproj
```

## Layer Responsibilities

### 1. **ProductManagement.Core** (Domain Layer)

- **Entities**: Core business models (`Product`, `SellRequest`, `PriceUpdateItem`)
- **Exceptions**: Custom domain exceptions for error handling
- **Validators**: Business rule validators using FluentValidation
- **No dependencies**: Pure domain logic, independent of external frameworks

### 2. **ProductManagement.Application** (Business Logic Layer)

- **Services**: Business use case implementations
  - `ProductService`: Product CRUD and search operations
  - `InventoryService`: Stock management
  - `PricingService`: Price update operations
- **DTOs**: Data Transfer Objects for API communication
- **Interfaces**: Service contracts for dependency injection
- **Dependencies**: Core layer only

### 3. **ProductManagement.Infrastructure** (Data Access Layer)

- **Repositories**: Data persistence implementations (`ProductRepository`)
- **In-memory storage**: Current implementation uses in-memory list (easily replaceable with EF Core)
- **Dependencies**: Application layer (implements interfaces)

### 4. **ProductManagement.Api** (Presentation Layer)

- **Controllers**: REST API endpoints (`ProductsController`)
- **Middleware**: Cross-cutting concerns (`ErrorHandlingMiddleware`)
- **DI Configuration**: Service registration in `Program.cs`
- **Dependencies**: All layers

## Key Features

### ✅ Comprehensive Validation

- Product name: 2-200 characters, non-whitespace
- SKU: 3-50 characters, uppercase+numbers+hyphens only, must be unique
- Price: > 0, max 999,999.99, max 2 decimal places
- Stock: 0 to int.MaxValue
- Category: Valid enum values only

### ✅ Edge Case Handling

- Negative/zero quantity checks in sell operations
- Insufficient stock detection
- Duplicate SKU prevention
- Invalid price validation
- Null/empty collection handling

### ✅ Error Handling

- Global exception middleware
- Custom domain exceptions
- Consistent error responses
- Status code mapping

### ✅ Service Architecture

- Single Responsibility Principle: Each service handles specific domain
- Dependency Injection: Loose coupling via interfaces
- Testability: Services depend on interfaces, not concrete implementations

## API Endpoints

All endpoints are now organized under `/api/products`:

### Products

- `GET /api/products` - Get all products (optional: `?category=Food`)
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/search?keyword=abc` - Search by name or SKU
- `POST /api/products` - Create new product
- `POST /api/products/sell` - Sell/reduce stock
- `PUT /api/products/bulk-price-update` - Bulk update prices

## Running the Application

### Prerequisites

- .NET 10.0 SDK
- Visual Studio 2022 or VS Code with C# extension

### Build and Run

```bash
# From backend directory
cd ProductManagement.Api

# Restore packages
dotnet restore

# Build
dotnet build

# Run
dotnet run

# The API will be available at:
# http://localhost:5129
# https://localhost:7286
```

### With Visual Studio

1. Open `ProductManagement.sln`
2. Set `ProductManagement.Api` as startup project
3. Press F5 to run with HTTPS
4. Or Ctrl+F5 for HTTP

## Testing Structure (Ready for Implementation)

Test projects should follow:

```
tests/
├── ProductManagement.Core.Tests/
│   ├── Validators/
│   ├── Entities/
│   └── ProductValidatorTests.cs
├── ProductManagement.Application.Tests/
│   ├── Services/
│   │   ├── ProductServiceTests.cs
│   │   ├── InventoryServiceTests.cs
│   │   └── PricingServiceTests.cs
│   └── DTOs/
└── ProductManagement.Api.Tests/
    ├── Controllers/
    └── Middleware/
```

## Next Steps

1. **Add Unit Tests** - Implement xUnit tests with Moq
2. **Migrate to Entity Framework Core** - Replace in-memory repository
3. **Add Logging** - Configure Serilog or similar
4. **Add Caching** - Implement distributed caching for performance
5. **API Documentation** - Configure Swagger/OpenAPI

## Architecture Benefits

✅ **Separation of Concerns**: Each layer has a specific responsibility  
✅ **Testability**: Easy to mock services and repositories  
✅ **Scalability**: Can easily replace implementations (e.g., in-memory to database)  
✅ **Maintainability**: Clear structure, easy to understand and modify  
✅ **Reusability**: DTOs and interfaces can be shared across projects  
✅ **Professional Standards**: Follows industry best practices
