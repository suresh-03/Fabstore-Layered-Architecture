using AutoMapper;
using Fabstore.Domain.Interfaces.IProduct;
using FabstoreWebApplication.ViewModels;
using FabstoreWebAppliction.ViewModels.Product;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FabstoreWebApplication.Controllers;

public class ProductController : Controller
    {
    private readonly ILogger<ProductController> _logger;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(ILogger<ProductController> logger, IProductService productService, IMapper mapper)
        {
        _logger = logger;
        _productService = productService;
        _mapper = mapper;
        }

    [HttpGet]
    public async Task<IActionResult> Index(string category)
        {
        try
            {

            var productsModel = await _productService.GetProductsAsync(category);
            List<ProductView> products = _mapper.Map<List<ProductView>>(productsModel);
            _logger.LogInformation($"Fetched all products. Count: {products.Count}");

            return View(products);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching products in Index method.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }




    [HttpGet]
    public async Task<IActionResult> Details(int id, string category)
        {


        try
            {
            var result = await _productService.GetProductDetailsAsync(category, id);

            if (!result.Success)
                {
                return BadRequest(result.Message);
                }
            var productView = _mapper.Map<ProductView>(result.Product);
            return View(productView);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error occurred while retrieving product details for ID: {ProductId}", id);
            return StatusCode(500, "An error occurred while processing your request.");
            }
        }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
        {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


    [HttpPost]
    [Route("/api/product/filter")]
    public async Task<IActionResult> FilterProducts([FromBody] Filter filterParams)
        {
        ViewData["Filter"] = "filter";
        try
            {


            // If no category provided, return all products
            var productModel = await _productService.GetProductsAsync(filterParams.Category);
            List<ProductView> products = _mapper.Map<List<ProductView>>(productModel);
            _logger.LogInformation(productModel.Count().ToString());
            products = FilterProducts(products, filterParams);

            _logger.LogInformation($"Fetched all products. Count: {products.Count}");

            return PartialView("_ProductList", products);
            }
        catch (Exception ex)
            {
            _logger.LogError(ex, "Error fetching products in Index method.");
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }


    private List<ProductView> FilterProducts(List<ProductView> products, Filter filterParams)
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

        if (!string.IsNullOrEmpty(filterParams.Sort))
            {
            if (filterParams.Sort.Equals("low-high"))
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

