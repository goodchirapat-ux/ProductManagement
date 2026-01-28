namespace ProductManagement.Application.Helpers;

using ProductManagement.Core.Entities;

public static class CategoryConverter
{
    public static ProductCategory StringToCategory(string categoryString)
    {
        if (string.IsNullOrWhiteSpace(categoryString))
            throw new ArgumentException("Category cannot be empty", nameof(categoryString));

        if (Enum.TryParse<ProductCategory>(categoryString, ignoreCase: true, out var category))
            return category;

        throw new ArgumentException($"Invalid category: {categoryString}. Valid categories are: {string.Join(", ", Enum.GetNames(typeof(ProductCategory)))}", nameof(categoryString));
    }

    public static string CategoryToString(ProductCategory category)
    {
        return category.ToString();
    }
}
