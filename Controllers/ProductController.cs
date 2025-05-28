using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using FabstoreWebAppliction.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Controllers;

// Controller for handling product-related actions
public class ProductController : Controller
    {
    // Logger for logging product events and errors
    private readonly ILogger<ProductController> _logger;
    // Service for product-related business logic
    private readonly IProductService _productService;

    // Constructor with dependency injection
    public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
        _logger = logger;
        _productService = productService;
        }

    // Displays the product list for a given category
    [HttpGet]
    public async Task<IActionResult> Index(string category)
        {
        try
            {
            // Fetch products for the specified category
            var serviceResponse = await _productService.GetProductsAsync(category);

            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }
            return View(serviceResponse.Data);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching products in Index method.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while getting products.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    // Displays the details of a specific product
    [HttpGet]
    public async Task<IActionResult> Details(int id, string category)
        {
        try
            {
            // Fetch product details by category and ID
            var serviceResponse = await _productService.GetProductDetailsAsync(category, id);

            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }

            return View(serviceResponse.Data);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving product details for ID: {ProductId}", id);
            return ResponseFilter.HandleResponse(false, "Something went wrong while getting product details", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    // Filters products based on the provided filter parameters and returns a partial view
    [HttpPost]
    [Route("/api/product/filter")]
    public async Task<IActionResult> FilterProducts([FromBody] Filter filterParams)
        {
        try
            {
            ViewData["Filter"] = "filter";

            // Fetch products for the specified category
            var serviceResponse = await _productService.GetProductsAsync(filterParams.Category);

            if (!serviceResponse.Success)
                {
                return ResponseFilter.HandleResponse(serviceResponse);
                }

            // Apply additional filtering and sorting
            var productModel = FilterProducts(serviceResponse.Data, filterParams);

            _logger.LogInformation($"Fetched all products. Count: {productModel.Count}");

            return PartialView("_ProductList", productModel);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching products in Index method.");
            return ResponseFilter.HandleResponse(false, "Something went wrong while filtering products.", HttpStatusCode.INTERNAL_SERVER_ERROR);
            }
        }

    // Helper method to filter and sort products based on filter parameters
    private List<Product> FilterProducts(List<Product> products, Filter filterParams)
        {
        var filteredProducts = products.AsParallel();

        // Filter by color if specified
        if (!filterParams.Color.Equals("all"))
            {
            filteredProducts = filteredProducts.Where(p => p.Variants.Any(v => v.Color == filterParams.Color));
            }
        // Filter by brand if specified
        if (!filterParams.Brand.Equals("all"))
            {
            filteredProducts = filteredProducts.Where(p => p.Brand.BrandName == filterParams.Brand);
            }
        // Filter by price range
        filteredProducts = filteredProducts.Where(p => p.Variants.Any(v => v.Price >= filterParams.MinPrice && v.Price <= filterParams.MaxPrice));

        var filteredList = filteredProducts.ToList();

        // Sort by average rating if specified
        if (!string.IsNullOrEmpty(filterParams.SortRating))
            {
            if (filterParams.SortRating.Equals("low-high"))
                {
                filteredList = filteredList.OrderBy(p => p.Reviews.Average(r => r.Rating)).ToList();
                }
            else
                {
                filteredList = filteredList.OrderByDescending(p => p.Reviews.Average(r => r.Rating)).ToList();
                }
            }

        // Sort by price if specified
        if (!string.IsNullOrEmpty(filterParams.SortPrice))
            {
            if (filterParams.SortPrice.Equals("low-high"))
                {
                filteredList = filteredList.OrderBy(p => p.Variants.Min(v => v.Price)).ToList();
                }
            else
                {
                filteredList = filteredList.OrderByDescending(p => p.Variants.Max(v => v.Price)).ToList();
                }
            }

        return filteredList;
        }
    }