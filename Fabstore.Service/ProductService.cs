using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using System.Collections.Concurrent;
using static Fabstore.Framework.CommonEnums;

namespace Fabstore.Service;

public class ProductService : IProductService
    {

    private readonly IProductRepository _repo;

    public ProductService(IProductRepository repo)
        {
        _repo = repo;
        }



    public async Task<List<Product>> GetProductsAsync(string? categoy)
        {
        return await _repo.GetProductsAsync(categoy);
        }


    public async Task<(bool Success, string? Message, Product? Product)> GetProductDetailsAsync(string? category, int id)
        {
        if (id <= 0)
            {
            return (false, $"Invalid product ID: {id}", null);

            }

        if (string.IsNullOrWhiteSpace(category))
            {
            return (false, $"Missing category for product ID: {id}", null);
            }

        var product = await _repo.GetProductDetailsAsync(category, id);
        if (product == null)
            {
            return (false, $"Product not found in category {category} with ID: {id}", null);
            }

        return (true, "Product Fetched Successfully", product);
        }

    public async Task<(bool Success, string Message, List<Product>? SearchedProducts)> GetSearchedProductsAsync(string query)
        {
        var products = await _repo.GetSearchedProductsAsync();

        var matchedProducts = GetMatchedProducts(query, products);

        if (matchedProducts.Count == 0)
            {
            return (false, "No products found", null);
            }

        return (true, "Products Fetched Successfully", matchedProducts);

        }

    private List<Product> GetMatchedProducts(string query, List<Product> products)
        {
        string[] tokens = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        ConcurrentBag<Product> matchedProducts = new ConcurrentBag<Product>();

        Parallel.ForEach(products, product =>
        {
            int matchCount = 0;
            foreach (var token in tokens)
                {
                if (ProductMatch(product, token))
                    {
                    matchCount++;
                    }
                }

            double matchPercentage = (double)matchCount / tokens.Length * 100;
            if (matchPercentage >= 75)
                {
                matchedProducts.Add(product);
                }
            // DEBUG
            //Console.WriteLine($"Search Controller {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.Name}");
        });

        return matchedProducts.ToList();
        }

    private bool ProductMatch(Product product, string token)
        {
        return (product.Brand?.BrandName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Category?.ParentCategory?.CategoryName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Category?.CategoryName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.ProductName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Description?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Variants?.Any(v => v.Color?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ?? false);
        }

    public async Task<(bool Success, string Message, Dictionary<string, List<string>> Categories)> GetCategoriesAsync()
        {
        var categories = await _repo.GetCategoriesAsync();


        if (categories != null)
            {
            var categoriesDictionary = categories
                  .AsParallel()
                  .GroupBy(c => c.ParentCategoryID)
                  .ToDictionary(
                      g => ((GenderType)g.Key).ToString(),
                      g => g.AsParallel().Select(c => c.CategoryName).ToList()
                  );

            return (true, "Categories fetched", categoriesDictionary);
            }
        else
            {
            return (false, "No categories found", null);
            }
        }
    }

