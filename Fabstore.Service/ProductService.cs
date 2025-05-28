using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;
using Fabstore.Service.Constants;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using static Fabstore.Framework.CommonEnums;

namespace Fabstore.Service;

// Service implementation for product-related business logic
public class ProductService : IProductService
    {

    // Repository for product data access
    private readonly IProductRepository _productRepository;
    // Factory for creating standardized service responses
    private readonly IServiceResponseFactory _responseFactory;
    // Memory cache for caching frequently accessed data
    private readonly IMemoryCache _memoryCache;

    // Constructor with dependency injection for repository, response factory, and memory cache
    public ProductService(IProductRepository productRepository, IServiceResponseFactory responseFactory, IMemoryCache memoryCache)
        {
        _productRepository = productRepository;
        _responseFactory = responseFactory;
        _memoryCache = memoryCache;
        }

    // Retrieves a list of products, optionally filtered by category
    public async Task<IServiceResponse<List<Product>>> GetProductsAsync(string? categoy)
        {
        try
            {
            var products = await _productRepository.GetProductsAsync(categoy);
            return _responseFactory.CreateResponse<List<Product>>(true, "Products Fetched Successfully", ActionType.Retrieved, products);
            }
        catch (Exception ex)
            {
            throw new ServiceException("Error occurred while getting products", ex);
            }
        }

    // Retrieves the details of a specific product by category and ID
    public async Task<IServiceResponse<Product>> GetProductDetailsAsync(string? category, int id)
        {
        try
            {
            // Validate product ID
            if (id <= 0)
                {
                return _responseFactory.CreateResponse<Product>(false, $"Invalid product ID: {id}", ActionType.ValidationError, null);
                }

            // Validate category
            if (string.IsNullOrWhiteSpace(category))
                {
                return _responseFactory.CreateResponse<Product>(false, $"Missing category for product ID: {id}", ActionType.ValidationError, null);
                }

            var product = await _productRepository.GetProductDetailsAsync(category, id);
            if (product == null)
                {
                // Product not found
                return _responseFactory.CreateResponse<Product>(false, $"Product not found in category {category} with ID: {id}", ActionType.NotFound, null);
                }
            return _responseFactory.CreateResponse<Product>(true, "Product Fetched Successfully", ActionType.Retrieved, product);
            }
        catch (Exception ex)
            {
            throw new ServiceException($"Error occurred while getting product details for category {category} and ID {id}", ex);
            }
        }

    // Retrieves a list of products matching the search query
    public async Task<IServiceResponse<List<Product>>> GetSearchedProductsAsync(string query)
        {
        try
            {
            var products = await _productRepository.GetSearchedProductsAsync();

            // Filter products based on the search query
            var matchedProducts = GetMatchedProducts(query, products);

            if (matchedProducts.Count == 0)
                {
                // No products matched the search query
                return _responseFactory.CreateResponse<List<Product>>(true, "No products found", ActionType.Retrieved, null);
                }

            return _responseFactory.CreateResponse<List<Product>>(true, "Products Fetched Successfully", ActionType.Retrieved, matchedProducts);
            }
        catch (Exception ex)
            {
            throw new ServiceException($"Error occurred while searching for products with query '{query}'", ex);
            }
        }

    // Helper method to filter products based on search tokens
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

            // Add product if at least 75% of tokens match
            double matchPercentage = (double)matchCount / tokens.Length * 100;
            if (matchPercentage >= 75)
                {
                matchedProducts.Add(product);
                }
        });

        return matchedProducts.ToList();
        }

    // Checks if a product matches a given search token
    private bool ProductMatch(Product product, string token)
        {
        return (product.Brand?.BrandName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Category?.ParentCategory?.CategoryName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Category?.CategoryName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.ProductName?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Description?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ||
               (product.Variants?.Any(v => v.Color?.Contains(token, StringComparison.OrdinalIgnoreCase) ?? false) ?? false);
        }

    // Retrieves categories, using cache if available, otherwise fetches from repository
    public async Task<IServiceResponse<Dictionary<string, List<string>>>> GetCategoriesAsync()
        {
        try
            {
            List<Category> categories;

            // Check if categories are cached, otherwise fetch from repository
            if (!_memoryCache.TryGetValue(CacheKeys.CATEGORIES, out categories))
                {
                categories = await _productRepository.GetCategoriesAsync();
                if (categories != null)
                    {
                    var cacheEntryOptions = new MemoryCacheEntryOptions
                        {
                        SlidingExpiration = TimeSpan.FromMinutes(10)
                        };

                    // Store categories in cache
                    _memoryCache.Set(CacheKeys.CATEGORIES, categories, cacheEntryOptions);
                    }
                }

            if (categories != null)
                {
                // Group categories by parent category and convert to dictionary
                var categoriesDictionary = categories
                      .AsParallel()
                      .GroupBy(c => c.ParentCategoryID)
                      .ToDictionary(
                          g => ((GenderType)g.Key).ToString(),
                          g => g.AsParallel().Select(c => c.CategoryName).ToList()
                      );

                return _responseFactory.CreateResponse<Dictionary<string, List<string>>>(true, "Categories fetched", ActionType.Retrieved, categoriesDictionary);
                }
            else
                {
                // No categories found
                return _responseFactory.CreateResponse<Dictionary<string, List<string>>>(false, "No categories found", ActionType.NotFound, null);
                }
            }
        catch (Exception ex)
            {
            throw new ServiceException("Error occurred while getting categories", ex);
            }
        }
    }