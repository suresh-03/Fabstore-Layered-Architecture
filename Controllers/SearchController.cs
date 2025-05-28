using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Controllers
    {
    // Controller for handling product search functionality
    public class SearchController : Controller
        {

        // Logger for logging search events and errors
        private readonly ILogger<SearchController> _logger;
        // Service for product-related business logic
        private readonly IProductService _productService;

        // Constructor with dependency injection
        public SearchController(ILogger<SearchController> logger, IProductService productService)
            {
            _logger = logger;
            _productService = productService;
            }

        // Not used for routing, but required by MVC conventions
        [NonAction]
        public IActionResult Index()
            {
            return View();
            }

        // Handles product search requests via API
        [HttpGet]
        [Route("api/search")]
        public async Task<IActionResult> Search([FromQuery] string query)
            {
            // Validate the search query
            if (string.IsNullOrWhiteSpace(query))
                {
                _logger.LogWarning("Empty search query received.");
                return ResponseFilter.HandleResponse(false, "Invalid Query", HttpStatusCode.BAD_REQUEST);
                }

            // Clean/filter the search query string
            string filteredQuery = StringHelper.FilterQuery(query);

            // Call the product service to get matching products
            var serviceResponse = await _productService.GetSearchedProductsAsync(filteredQuery);

            // Handle unsuccessful search response
            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }

            // Pass the original query to the view for display
            ViewData["Query"] = query;
            // Return the partial view with the search results
            return PartialView("_ProductList", serviceResponse.Data);
            }

        }
    }