using Fabstore.Domain.Interfaces.IProduct;
using Fabstore.WebApplication.Constants;
using Fabstore.WebApplication.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Controllers
    {
    public class SearchController : Controller
        {

        private readonly ILogger<SearchController> _logger;
        private readonly IProductService _productService;

        public SearchController(ILogger<SearchController> logger, IProductService productService)
            {
            _logger = logger;
            _productService = productService;
            }

        [NonAction]
        public IActionResult Index()
            {
            return View();
            }

        [HttpGet]
        [Route("api/search")]
        public async Task<IActionResult> Search([FromQuery] string query)
            {
            if (string.IsNullOrWhiteSpace(query))
                {
                _logger.LogWarning("Empty search query received.");
                return ResponseFilter.HandleResponse(false, "Invalid Query", HttpStatusCode.BAD_REQUEST);
                }

            string filteredQuery = StringHelper.FilterQuery(query);

            var serviceResponse = await _productService.GetSearchedProductsAsync(filteredQuery);

            if (!serviceResponse.Success)
                {
                _logger.LogWarning(serviceResponse.Message);
                return ResponseFilter.HandleResponse(serviceResponse);
                }

            ViewData["Query"] = query;
            return PartialView("_ProductList", serviceResponse.Data);
            }

        }
    }
