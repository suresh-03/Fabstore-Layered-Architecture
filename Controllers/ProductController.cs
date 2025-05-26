using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.Domain.Models;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using FabstoreWebAppliction.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Controllers;

public class ProductController : Controller
    {
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;


    public ProductController(ILogger<ProductController> logger, IProductService productService)
        {
        _logger = logger;
        _productService = productService;
        }

    [HttpGet]
    public async Task<IActionResult> Index(string category)
        {
        try
            {

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




    [HttpGet]
    public async Task<IActionResult> Details(int id, string category)
        {
        try
            {
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

    [HttpPost]
    [Route("/api/product/filter")]
    public async Task<IActionResult> FilterProducts([FromBody] Filter filterParams)
        {
        try
            {

            ViewData["Filter"] = "filter";

            var serviceResponse = await _productService.GetProductsAsync(filterParams.Category);

            if (!serviceResponse.Success)
                {
                return ResponseFilter.HandleResponse(serviceResponse);
                }

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


    private List<Product> FilterProducts(List<Product> products, Filter filterParams)
        {
        // DEBUG
        //var filteredProducts = products.AsEnumerable();

        var filteredProducts = products.AsParallel();

        if (!filterParams.Color.Equals("all"))
            {

            filteredProducts = filteredProducts.Where(p => p.Variants.Any(v => v.Color == filterParams.Color));

            // DEBUG
            //filteredProducts = filteredProducts.Where(p =>
            //{
            //    Console.WriteLine($"Color Filter {p.ProductName} {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.Name}");
            //    return p.Variants.Any(v => v.Color == filterParams.Color);
            //});

            }
        if (!filterParams.Brand.Equals("all"))
            {
            filteredProducts = filteredProducts.Where(p => p.Brand.BrandName == filterParams.Brand);
            }
        filteredProducts = filteredProducts.Where(p => p.Variants.Any(v => v.Price >= filterParams.MinPrice && v.Price <= filterParams.MaxPrice));

        var filteredList = filteredProducts.ToList();

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

