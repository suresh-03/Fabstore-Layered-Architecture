using Fabstore.Domain.CustomExceptions;
using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Fabstore.Domain.ResponseFormat;
using System.Collections.Concurrent;
using static Fabstore.Framework.CommonEnums;

namespace Fabstore.Service;

public class ProductService : IProductService
    {

    private readonly IProductRepository _productRepository;
    private readonly IServiceResponseFactory _responseFactory;

    public ProductService(IProductRepository productRepository, IServiceResponseFactory responseFactory)
        {
        _productRepository = productRepository;
        _responseFactory = responseFactory;
        }



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


    public async Task<IServiceResponse<Product>> GetProductDetailsAsync(string? category, int id)
        {
        try
            {
            if (id <= 0)
                {
                return _responseFactory.CreateResponse<Product>(false, $"Invalid product ID: {id}", ActionType.ValidationError, null);

                }

            if (string.IsNullOrWhiteSpace(category))
                {
                return _responseFactory.CreateResponse<Product>(false, $"Missing category for product ID: {id}", ActionType.ValidationError, null);
                }

            var product = await _productRepository.GetProductDetailsAsync(category, id);
            if (product == null)
                {
                return _responseFactory.CreateResponse<Product>(false, $"Product not found in category {category} with ID: {id}", ActionType.NotFound, null);
                }
            return _responseFactory.CreateResponse<Product>(true, "Product Fetched Successfully", ActionType.Retrieved, product);

            }
        catch (Exception ex)
            {
            throw new ServiceException($"Error occurred while getting product details for category {category} and ID {id}", ex);
            }

        }

    public async Task<IServiceResponse<List<Product>>> GetSearchedProductsAsync(string query)
        {
        try
            {
            var products = await _productRepository.GetSearchedProductsAsync();

            var matchedProducts = GetMatchedProducts(query, products);

            if (matchedProducts.Count == 0)
                {
                return _responseFactory.CreateResponse<List<Product>>(true, "No products found", ActionType.Retrieved, null);
                }

            return _responseFactory.CreateResponse<List<Product>>(true, "Products Fetched Successfully", ActionType.Retrieved, matchedProducts);

            }
        catch (Exception ex)
            {
            throw new ServiceException($"Error occurred while searching for products with query '{query}'", ex);
            }
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

    public async Task<IServiceResponse<Dictionary<string, List<string>>>> GetCategoriesAsync()
        {
        try
            {
            var categories = await _productRepository.GetCategoriesAsync();


            if (categories != null)
                {
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
                return _responseFactory.CreateResponse<Dictionary<string, List<string>>>(false, "No categories found", ActionType.NotFound, null);
                }
            }
        catch (Exception ex)
            {
            throw new ServiceException("Error occurred while getting categories", ex);
            }
        }
    }

