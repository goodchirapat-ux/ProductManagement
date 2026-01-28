# FlowAccount Project Enhancement Guide

## Overview

This document outlines professional enhancements across three key areas: Architecture & Organization, Unit Testing, and Edge Case Handling.

---

## 1. ARCHITECTURE & ORGANIZATION ENHANCEMENTS

### 1.1 Backend Folder Structure (C# - .NET 10)

#### Current State ❌

```
backend/
└── ProductManagement/
    ├── Program.cs (144 lines, mixed concerns)
    ├── entity/
    │   ├── Product.cs
    │   ├── PriceUpdateItem.cs
    │   └── SellReq.cs
    └── Properties/
```

#### Recommended Structure ✅

```
backend/
├── FlowAccount.Backend.sln
├── src/
│   ├── FlowAccount.Core/                    # Domain models & business logic
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   ├── ProductCategory.cs
│   │   │   └── PriceUpdateItem.cs
│   │   ├── ValueObjects/
│   │   │   ├── Sku.cs
│   │   │   └── Money.cs
│   │   ├── Exceptions/
│   │   │   ├── ProductNotFoundException.cs
│   │   │   ├── InsufficientStockException.cs
│   │   │   └── DomainException.cs
│   │   └── Validators/
│   │       └── ProductValidator.cs
│   │
│   ├── FlowAccount.Application/            # Business use cases/services
│   │   ├── Services/
│   │   │   ├── ProductService.cs
│   │   │   ├── InventoryService.cs
│   │   │   └── PricingService.cs
│   │   ├── DTOs/
│   │   │   ├── CreateProductDto.cs
│   │   │   ├── SellProductDto.cs
│   │   │   ├── BulkPriceUpdateDto.cs
│   │   │   └── ProductResponseDto.cs
│   │   ├── Mappers/
│   │   │   └── ProductMapper.cs
│   │   └── Interfaces/
│   │       ├── IProductService.cs
│   │       ├── IInventoryService.cs
│   │       └── IPricingService.cs
│   │
│   ├── FlowAccount.Infrastructure/         # Data access, external services
│   │   ├── Repositories/
│   │   │   ├── IProductRepository.cs
│   │   │   └── ProductRepository.cs
│   │   ├── Persistence/
│   │   │   └── InMemoryProductStore.cs (for now, later replace with EF Core)
│   │   └── ServiceCollectionExtensions.cs
│   │
│   └── FlowAccount.Api/                    # API layer (minimal)
│       ├── Program.cs                      # DI & middleware setup only
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Properties/
│       │   └── launchSettings.json
│       ├── Controllers/
│       │   ├── ProductsController.cs
│       │   ├── InventoryController.cs
│       │   └── PricingController.cs
│       ├── Middleware/
│       │   ├── ErrorHandlingMiddleware.cs
│       │   └── ValidationMiddleware.cs
│       ├── Filters/
│       │   └── ApiExceptionFilter.cs
│       └── Configuration/
│           └── ServiceConfiguration.cs
│
└── tests/
    ├── FlowAccount.Core.Tests/
    │   ├── Entities/
    │   │   ├── ProductTests.cs
    │   │   └── ProductCategoryTests.cs
    │   ├── Validators/
    │   │   └── ProductValidatorTests.cs
    │   └── ValueObjects/
    │       └── SkuTests.cs
    │
    ├── FlowAccount.Application.Tests/
    │   ├── Services/
    │   │   ├── ProductServiceTests.cs
    │   │   ├── InventoryServiceTests.cs
    │   │   └── PricingServiceTests.cs
    │   └── Mappers/
    │       └── ProductMapperTests.cs
    │
    └── FlowAccount.Api.Tests/
        ├── Controllers/
        │   ├── ProductsControllerTests.cs
        │   └── InventoryControllerTests.cs
        └── Middleware/
            └── ErrorHandlingMiddlewareTests.cs
```

### 1.2 Frontend Folder Structure (Angular)

#### Current State ❌

```
frontend/
└── ProductManagementUI/
    └── src/app/
        ├── components/
        │   └── product-list/
        ├── models/
        │   └── product.ts
        └── app.ts
```

#### Recommended Structure ✅

```
frontend/
└── ProductManagementUI/
    └── src/
        ├── app/
        │   ├── core/                       # Singleton services, guards
        │   │   ├── services/
        │   │   │   ├── product.service.ts
        │   │   │   ├── inventory.service.ts
        │   │   │   ├── api.service.ts
        │   │   │   └── error-handler.service.ts
        │   │   ├── guards/
        │   │   │   └── auth.guard.ts
        │   │   └── interceptors/
        │   │       └── error.interceptor.ts
        │   │
        │   ├── shared/                     # Reusable components, pipes, directives
        │   │   ├── components/
        │   │   │   ├── loading-spinner/
        │   │   │   ├── error-alert/
        │   │   │   └── confirmation-dialog/
        │   │   ├── pipes/
        │   │   │   ├── currency-format.pipe.ts
        │   │   │   └── text-truncate.pipe.ts
        │   │   ├── directives/
        │   │   │   └── highlight-errors.directive.ts
        │   │   └── models/
        │   │       ├── api-response.ts
        │   │       ├── api-error.ts
        │   │       └── pagination.ts
        │   │
        │   ├── features/                   # Feature modules
        │   │   ├── products/
        │   │   │   ├── models/
        │   │   │   │   ├── product.ts
        │   │   │   │   └── product-filter.ts
        │   │   │   ├── services/
        │   │   │   │   └── product-facade.service.ts
        │   │   │   ├── components/
        │   │   │   │   ├── product-list/
        │   │   │   │   ├── product-detail/
        │   │   │   │   ├── product-form/
        │   │   │   │   └── product-search/
        │   │   │   ├── pages/
        │   │   │   │   └── products.page.ts
        │   │   │   ├── products.module.ts (or routes if standalone)
        │   │   │   └── products.routes.ts
        │   │   │
        │   │   └── inventory/
        │   │       ├── components/
        │   │       ├── pages/
        │   │       ├── models/
        │   │       ├── services/
        │   │       └── inventory.routes.ts
        │   │
        │   ├── app.config.ts              # DI configuration
        │   ├── app.routes.ts              # Main routes
        │   ├── app.ts                     # Root component
        │   └── app.css
        │
        ├── assets/
        │   └── styles/
        │       ├── variables.css
        │       ├── global.css
        │       └── utilities.css
        │
        └── environments/
            ├── environment.ts
            └── environment.prod.ts
```

### 1.3 Naming Conventions

#### Backend C#

| Item           | Convention              | Example                                                         |
| -------------- | ----------------------- | --------------------------------------------------------------- |
| Classes        | PascalCase              | `ProductService`, `IProductRepository`                          |
| Interfaces     | IPascalCase             | `IProductService`, `IRepository`                                |
| Methods        | PascalCase              | `CreateProduct()`, `GetByIdAsync()`                             |
| Properties     | PascalCase              | `Id`, `ProductName`                                             |
| Private fields | \_camelCase             | `_repository`, `_logger`                                        |
| Constants      | UPPER_SNAKE_CASE        | `MAX_PRICE`, `DEFAULT_QUANTITY`                                 |
| Namespaces     | PascalCase.Hierarchical | `FlowAccount.Core.Entities`, `FlowAccount.Application.Services` |
| Files          | Match class name        | `ProductService.cs`                                             |

#### Frontend TypeScript/Angular

| Item        | Convention            | Example                                           |
| ----------- | --------------------- | ------------------------------------------------- |
| Components  | PascalCase (exported) | `ProductListComponent`                            |
| Services    | PascalCase + Service  | `ProductService`, `InventoryService`              |
| Interfaces  | IPascalCase           | `IProduct`, `IApiResponse`                        |
| Type/Models | PascalCase            | `Product`, `ProductFilter`                        |
| Functions   | camelCase             | `getProductById()`, `calculatePrice()`            |
| Variables   | camelCase             | `productList`, `currentFilter`                    |
| Constants   | UPPER_SNAKE_CASE      | `MAX_PRODUCTS_PER_PAGE`, `API_TIMEOUT`            |
| File names  | kebab-case            | `product-list.component.ts`, `product.service.ts` |
| Folders     | kebab-case            | `product-list/`, `shared/`                        |

---

## 2. UNIT TESTING STRATEGY

### 2.1 Backend Testing (.NET)

#### Dependencies to Add

```xml
<PackageReference Include="xUnit" Version="2.6.0" />
<PackageReference Include="xUnit.Runner.VisualStudio" Version="2.5.0" />
<PackageReference Include="Moq" Version="4.20.0" />
<PackageReference Include="FluentAssertions" Version="6.12.0" />
<PackageReference Include="Testcontainers" Version="3.7.0" />
```

#### Test Structure

**1. Unit Tests - ProductValidator.cs**

```csharp
public class ProductValidatorTests
{
    private ProductValidator _validator;
    private List<Product> _existingProducts;

    [Fact]
    public void Validate_WithValidProduct_ReturnsSuccess()
    {
        // Arrange
        var product = new Product(1, "Valid Product", "SKU123", 99.99m, 10, ProductCategory.Food);
        _validator = new ProductValidator(_existingProducts);

        // Act
        var result = _validator.Validate(product);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithEmptyName_ReturnsFail(string name)
    {
        // Test cases for empty names
    }

    [Fact]
    public void Validate_WithDuplicateSku_ReturnsFail()
    {
        // Test duplicate SKU detection
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_WithNegativePrice_ReturnsFail(decimal price)
    {
        // Test negative prices
    }
}
```

**2. Service Tests - ProductService.cs**

```csharp
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly ProductService _service;

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsSavedProduct()
    {
        // Arrange
        var createDto = new CreateProductDto(...);
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync(new Product(...));

        // Act
        var result = await _service.CreateProductAsync(createDto);

        // Assert
        result.Should().NotBeNull();
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task SellProduct_WithInsufficientStock_ThrowsException()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<InsufficientStockException>(
            () => _service.SellAsync(productId, excessiveQuantity));
    }
}
```

**3. Integration Tests - ProductsControllerTests.cs**

```csharp
public class ProductsControllerIntegrationTests : IAsyncLifetime
{
    private WebApplicationFactory<Program> _factory;
    private HttpClient _client;

    [Fact]
    public async Task GetProducts_ReturnsOkWithProductList()
    {
        // Arrange
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/products");

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await response.Content.ReadAsAsync<List<ProductDto>>();
        content.Should().NotBeNull();
    }
}
```

### 2.2 Frontend Testing (Angular/Jasmine)

#### Dependencies

```json
{
  "devDependencies": {
    "jasmine-core": "~5.0.0",
    "karma": "~6.4.0",
    "karma-jasmine": "~5.1.0",
    "@angular/core/testing": "^18.0.0"
  }
}
```

#### Test Examples

**1. Service Tests - ProductService.spec.ts**

```typescript
describe("ProductService", () => {
  let service: ProductService;
  let httpClient: HttpClient;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [ProductService],
    });
    service = TestBed.inject(ProductService);
    httpClient = TestBed.inject(HttpClient);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  it("should fetch products", () => {
    const mockProducts = [
      { id: 1, name: "Product 1", price: 10 },
      { id: 2, name: "Product 2", price: 20 },
    ];

    service.getProducts().subscribe((products) => {
      expect(products.length).toBe(2);
      expect(products).toEqual(mockProducts);
    });

    const req = httpTestingController.expectOne("/api/products");
    expect(req.request.method).toBe("GET");
    req.flush(mockProducts);
  });

  afterEach(() => {
    httpTestingController.verify();
  });
});
```

**2. Component Tests - ProductListComponent.spec.ts**

```typescript
describe("ProductListComponent", () => {
  let component: ProductListComponent;
  let fixture: ComponentFixture<ProductListComponent>;
  let productService: jasmine.SpyObj<ProductService>;

  beforeEach(async () => {
    const spy = jasmine.createSpyObj("ProductService", ["getProducts"]);

    await TestBed.configureTestingModule({
      imports: [ProductListComponent],
      providers: [{ provide: ProductService, useValue: spy }],
    }).compileComponents();

    productService = TestBed.inject(ProductService) as jasmine.SpyObj<ProductService>;
    fixture = TestBed.createComponent(ProductListComponent);
    component = fixture.componentInstance;
  });

  it("should display products", fakeAsync(() => {
    const mockProducts = [{ id: 1, name: "Test", price: 10 }];
    productService.getProducts.and.returnValue(of(mockProducts));

    fixture.detectChanges();
    tick();

    expect(component.products()).toEqual(mockProducts);
  }));
});
```

---

## 3. COMPREHENSIVE EDGE CASE HANDLING

### 3.1 Backend Edge Cases

#### A. Data Validation Edge Cases

```csharp
public class ProductValidator : AbstractValidator<Product>
{
    public ProductValidator(IProductRepository repository)
    {
        // Price edge cases
        RuleFor(p => p.Price)
            .NotEmpty()
            .GreaterThan(0).WithMessage("Price must be greater than 0")
            .LessThanOrEqualTo(999999.99m).WithMessage("Price exceeds maximum allowed value")
            .Must(p => p % 0.01m == 0).WithMessage("Price can only have up to 2 decimal places");

        // Stock edge cases
        RuleFor(p => p.Stock)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative")
            .LessThanOrEqualTo(int.MaxValue).WithMessage("Stock exceeds maximum allowed value");

        // SKU edge cases
        RuleFor(p => p.Sku)
            .NotEmpty()
            .MinimumLength(3).WithMessage("SKU must be at least 3 characters")
            .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters")
            .Matches(@"^[A-Z0-9\-_]+$").WithMessage("SKU can only contain uppercase letters, numbers, hyphens, and underscores")
            .Must((product, sku) => !repository.SkuExists(sku, product.Id))
            .WithMessage("SKU must be unique");

        // Name edge cases
        RuleFor(p => p.Name)
            .NotEmpty()
            .MinimumLength(2).WithMessage("Name must be at least 2 characters")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters")
            .Must(n => !string.IsNullOrWhiteSpace(n))
            .WithMessage("Name cannot be only whitespace");
    }
}
```

#### B. Business Logic Edge Cases

```csharp
public class InventoryService : IInventoryService
{
    public async Task<SellResult> SellProductAsync(int productId, int quantity)
    {
        // Edge case: quantity validation
        if (quantity <= 0)
            throw new InvalidOperationException("Quantity must be positive");

        if (quantity > int.MaxValue)
            throw new InvalidOperationException("Quantity exceeds allowed value");

        var product = await _repository.GetByIdAsync(productId);

        // Edge case: product not found
        if (product == null)
            throw new ProductNotFoundException($"Product {productId} not found");

        // Edge case: exact stock match
        if (product.Stock == quantity)
        {
            // Handle edge case: selling entire inventory
            return await _repository.UpdateStockAsync(productId, 0);
        }

        // Edge case: insufficient stock
        if (product.Stock < quantity)
            throw new InsufficientStockException(
                $"Only {product.Stock} units available, but {quantity} requested");

        var newStock = product.Stock - quantity;
        return await _repository.UpdateStockAsync(productId, newStock);
    }

    public async Task<PriceUpdateResult> BulkPriceUpdateAsync(List<PriceUpdateDto> updates)
    {
        // Edge case: empty list
        if (updates == null || !updates.Any())
            return new PriceUpdateResult { UpdatedCount = 0, FailedCount = 0 };

        // Edge case: duplicate IDs in request
        var duplicateIds = updates.GroupBy(u => u.ProductId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateIds.Any())
            throw new InvalidOperationException(
                $"Duplicate product IDs in request: {string.Join(", ", duplicateIds)}");

        // Edge case: extreme values
        var invalidUpdates = updates.Where(u => u.NewPrice <= 0 || u.NewPrice > 999999.99m).ToList();
        if (invalidUpdates.Any())
            throw new InvalidOperationException("Some price updates are invalid");

        var result = new PriceUpdateResult();
        foreach (var update in updates)
        {
            try
            {
                await _repository.UpdatePriceAsync(update.ProductId, update.NewPrice);
                result.UpdatedCount++;
            }
            catch (ProductNotFoundException)
            {
                result.FailedCount++;
            }
        }

        return result;
    }
}
```

#### C. API Response Edge Cases

```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetProduct(int id)
    {
        // Edge case: invalid ID
        if (id <= 0)
            return BadRequest(new { error = "Product ID must be a positive integer" });

        try
        {
            var product = await _service.GetByIdAsync(id);
            return Ok(product);
        }
        catch (ProductNotFoundException ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPost("search")]
    public async Task<IActionResult> SearchProducts([FromQuery] string keyword,
        [FromQuery] int? page = null,
        [FromQuery] int? pageSize = null)
    {
        // Edge case: null/empty keyword
        if (string.IsNullOrWhiteSpace(keyword))
            return BadRequest(new { error = "Keyword cannot be empty" });

        // Edge case: pagination validation
        const int maxPageSize = 100;
        const int minPageSize = 1;

        if (pageSize.HasValue && (pageSize.Value < minPageSize || pageSize.Value > maxPageSize))
            return BadRequest(new { error = $"Page size must be between {minPageSize} and {maxPageSize}" });

        if (page.HasValue && page.Value < 1)
            return BadRequest(new { error = "Page number must be positive" });

        var results = await _service.SearchAsync(keyword, page ?? 1, pageSize ?? 10);
        return Ok(results);
    }
}
```

#### D. Exception Handling

```csharp
// Custom exceptions
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

// Global exception middleware
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { error = "", statusCode = StatusCodes.Status500InternalServerError };

        return exception switch
        {
            ProductNotFoundException =>
                HandleResponse(context, 404, exception.Message),
            InsufficientStockException =>
                HandleResponse(context, 400, exception.Message),
            ValidationException =>
                HandleResponse(context, 400, exception.Message),
            ArgumentException =>
                HandleResponse(context, 400, "Invalid argument: " + exception.Message),
            _ => HandleResponse(context, 500, "An unexpected error occurred")
        };
    }

    private static Task HandleResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new { error = message });
    }
}
```

### 3.2 Frontend Edge Cases

#### A. API Error Handling

```typescript
// error-handler.service.ts
@Injectable({ providedIn: "root" })
export class ErrorHandlerService {
  handleApiError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = "An unexpected error occurred";

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      switch (error.status) {
        case 0:
          errorMessage = "Unable to connect to the server";
          break;
        case 400:
          errorMessage = error.error?.error || "Bad request";
          break;
        case 404:
          errorMessage = error.error?.error || "Resource not found";
          break;
        case 409:
          errorMessage = "Conflict: " + (error.error?.error || "Resource already exists");
          break;
        case 422:
          errorMessage = "Validation failed: " + this.formatValidationErrors(error.error);
          break;
        case 500:
          errorMessage = "Server error: Please try again later";
          break;
        default:
          errorMessage = `HTTP Error: ${error.status}`;
      }
    }

    return throwError(() => new Error(errorMessage));
  }

  private formatValidationErrors(errors: any): string {
    if (Array.isArray(errors)) {
      return errors.map((e) => e.message).join("; ");
    }
    return JSON.stringify(errors);
  }
}
```

#### B. Form Input Validation Edge Cases

```typescript
// product-form.component.ts
export class ProductFormComponent implements OnInit {
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private productService: ProductService,
  ) {
    this.form = this.fb.group({
      name: [
        "",
        [
          Validators.required,
          Validators.minLength(2),
          Validators.maxLength(200),
          this.noWhitespaceValidator,
        ],
      ],
      price: [
        "",
        [
          Validators.required,
          Validators.pattern(/^\d+(\.\d{1,2})?$/), // Max 2 decimals
          this.positiveNumberValidator,
          this.maxPriceValidator,
        ],
      ],
      stock: [
        "",
        [
          Validators.required,
          Validators.pattern(/^\d+$/), // Integers only
          Validators.min(0),
          Validators.max(2147483647), // int.MaxValue
        ],
      ],
      sku: [
        "",
        [
          Validators.required,
          Validators.minLength(3),
          Validators.maxLength(50),
          Validators.pattern(/^[A-Z0-9\-_]+$/),
        ],
      ],
      category: ["", Validators.required],
    });
  }

  // Custom validators
  noWhitespaceValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    return /^\s+$/.test(control.value) ? { whitespace: true } : null;
  }

  positiveNumberValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const num = parseFloat(control.value);
    return isNaN(num) || num <= 0 ? { positiveNumber: true } : null;
  }

  maxPriceValidator(control: AbstractControl): ValidationErrors | null {
    if (!control.value) return null;
    const num = parseFloat(control.value);
    return num > 999999.99 ? { maxPrice: true } : null;
  }

  onSubmit(): void {
    if (!this.form.valid) {
      this.markFormGroupTouched(this.form);
      return;
    }

    this.productService.createProduct(this.form.value).subscribe({
      next: (result) => this.onSuccess(result),
      error: (error) => this.onError(error),
    });
  }

  private markFormGroupTouched(formGroup: FormGroup): void {
    Object.keys(formGroup.controls).forEach((key) => {
      const control = formGroup.get(key);
      control?.markAsTouched();
    });
  }
}
```

#### C. Component State Edge Cases

```typescript
@Component({
  selector: "app-product-list",
  templateUrl: "./product-list.component.html",
  styleUrl: "./product-list.component.css",
})
export class ProductListComponent implements OnInit, OnDestroy {
  products$ = signal<Product[]>([]);
  isLoading = signal(false);
  error$ = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    // Edge case: prevent multiple simultaneous requests
    if (this.isLoading()) return;

    this.isLoading.set(true);
    this.productService
      .getProducts()
      .pipe(
        timeout(30000), // Edge case: request timeout
        retry({ count: 2, delay: 1000 }), // Edge case: retry on failure
        catchError((error) => {
          this.error$.next("Failed to load products");
          console.error("Error loading products:", error);
          return of([]);
        }),
        finalize(() => this.isLoading.set(false)),
        takeUntil(this.destroy$), // Prevent memory leaks
      )
      .subscribe((products) => {
        // Edge case: empty list handling
        this.products$.set(products && products.length > 0 ? products : []);
      });
  }

  sellProduct(productId: number, quantity: number): void {
    // Edge case: validate input
    if (quantity <= 0) {
      this.error$.next("Quantity must be positive");
      return;
    }

    this.productService
      .sellProduct(productId, quantity)
      .pipe(
        timeout(15000),
        catchError((error) => {
          const errorMsg = error.error?.error || "Failed to sell product";
          this.error$.next(errorMsg);
          return throwError(() => error);
        }),
      )
      .subscribe(() => {
        this.loadProducts(); // Refresh list
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
```

#### D. HTTP Interceptor with Edge Cases

```typescript
@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private errorHandler: ErrorHandlerService,
    private router: Router,
  ) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // Edge case: avoid intercepting non-API requests
    if (!req.url.includes("/api/")) {
      return next.handle(req);
    }

    // Add timeout
    return next.handle(req).pipe(
      timeout(30000),
      retry({
        count: 1,
        delay: (error) => {
          // Only retry on specific error codes
          if (error instanceof TimeoutError || error.status === 0 || error.status === 502) {
            return timer(1000);
          }
          return throwError(() => error);
        },
      }),
      catchError((error: HttpErrorResponse) => {
        // Edge case: redirect on 401 (Unauthorized)
        if (error.status === 401) {
          this.router.navigate(["/login"]);
        }

        // Edge case: network error detection
        if (error.status === 0) {
          console.error("Network error - possibly offline");
        }

        return this.errorHandler.handleApiError(error);
      }),
    );
  }
}
```

---

## 4. IMPLEMENTATION PRIORITY

### Phase 1: Foundation (Weeks 1-2)

- [ ] Reorganize folder structure
- [ ] Set up DI containers
- [ ] Extract business logic into services
- [ ] Create custom exceptions

### Phase 2: Testing Infrastructure (Weeks 3-4)

- [ ] Add test project structure
- [ ] Set up test frameworks (xUnit, Jasmine)
- [ ] Write validator tests
- [ ] Write service tests

### Phase 3: Edge Cases (Weeks 5-6)

- [ ] Implement comprehensive validation
- [ ] Add error handling middleware
- [ ] Enhance API error responses
- [ ] Add frontend error handling

### Phase 4: Polish (Week 7+)

- [ ] Integration tests
- [ ] E2E tests
- [ ] Documentation
- [ ] Performance optimization

---

## 5. QUICK CHECKLIST

### Architecture

- [ ] Backend: Multi-layer structure (Api, Application, Core, Infrastructure)
- [ ] Frontend: Feature-based modules with shared utilities
- [ ] Consistent naming conventions across projects
- [ ] Dependency Injection properly configured

### Testing

- [ ] Unit test coverage > 80%
- [ ] Integration tests for critical paths
- [ ] Service and component tests
- [ ] Mock external dependencies

### Edge Cases

- [ ] Validate all inputs (min/max, type, format)
- [ ] Handle null/empty collections
- [ ] Implement retry logic with timeouts
- [ ] Custom exception handling
- [ ] API response standardization
- [ ] Prevent race conditions
- [ ] Graceful degradation on errors
