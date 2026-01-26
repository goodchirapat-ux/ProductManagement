using entity;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options => {
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200") // Angular URL
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors("AllowAngularApp");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var products = new List<Product>();


app.MapGet("/api/products", (ProductCategory? category) =>
{
    if (category.HasValue)
    {
        var filteredProducts = products.Where(p => p.Category == category.Value).ToList();
        return Results.Ok(filteredProducts);
    }

    return Results.Ok(products);
})
.WithName("GetProducts");

app.MapPost("/api/products", (Product newProduct) => 
{
    var validator = new ProductValidator(products);
    var validationResult = validator.Validate(newProduct);

    if (!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }));
    }

    var nextId = products.Count > 0 ? products.Max(p => p.Id) + 1 : 1;
    var product = newProduct with { Id = nextId };
    
    products.Add(product);
    return Results.Created($"/api/products/{nextId}", product);
})
.WithName("CreateProduct");

app.MapPost("/api/products/sell", (SellRequest request) =>
{

    if (request.Quantity <= 0)
    {
        return Results.BadRequest(new { Message = "Quantity must be greater than zero to sell." });
    }
    var product = products.FirstOrDefault(p => p.Id == request.ProductId);
    if (product is null)
    {
        return Results.NotFound(new { Message = "Product not found." });
    }
    if (product.Stock < request.Quantity)
    {
        return Results.BadRequest(new { Message = "Insufficient stock." });
    }
    var updatedProduct = product with { Stock = product.Stock - request.Quantity };
    int index = products.IndexOf(product);
    products[index] = updatedProduct;
    return Results.Ok(new { 
        Message = "Sell successful", 
        RemainingStock = updatedProduct.Stock,
        Product = updatedProduct 
    });
})
.WithName("SellProduct");

app.MapGet("/api/products/search", (string? keyword) =>
{
    // If no keyword is provided, return all products
    if (string.IsNullOrWhiteSpace(keyword))
    {
        return Results.Ok(products);
    }

    // Filter products where Name OR SKU contains the keyword
    var filteredResults = products.Where(p => 
        p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) || 
        p.Sku.Contains(keyword, StringComparison.OrdinalIgnoreCase)
    ).ToList();

    return Results.Ok(filteredResults);
})
.WithName("SearchProducts");

app.MapPut("/api/products/bulk-price-update", (List<PriceUpdateItem> updates) =>
{
    int updatedCount = 0;

    foreach (var update in updates)
    {
        // 1. Find the product
        var product = products.FirstOrDefault(p => p.Id == update.ProductId);
        
        if (product != null)
        {
            // 2. Validate the new price (must be > 0)
            if (update.NewPrice > 0)
            {
                // 3. Create updated record and replace in list
                var updatedProduct = product with { Price = update.NewPrice };
                int index = products.IndexOf(product);
                products[index] = updatedProduct;
                
                updatedCount++;
            }
        }
    }

    // 4. Return summary
    return Results.Ok(new 
    { 
        Message = "Bulk update completed", 
        UpdatedItems = updatedCount, 
        TotalRequested = updates.Count 
    });
})
.WithName("BulkPriceUpdate");

app.Run();


