using AutoMapper;
using Fabstore.Domain.Interfaces.IProduct;
using FabstoreWebApplication.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.Controllers
    {
    public class SearchController : Controller
        {

        private readonly ILogger<SearchController> _logger;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public SearchController(ILogger<SearchController> logger, IProductService productService, IMapper mapper)
            {
            _logger = logger;
            _productService = productService;
            _mapper = mapper;
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
                return BadRequest("Search query cannot be empty.");
                }

            string filteredQuery = StringHelper.FilterQuery(query);

            var result = await _productService.GetSearchedProductsAsync(filteredQuery);

            var productsView = _mapper.Map<List<ProductView>>(result.SearchedProducts);

            _logger.LogInformation($"Fetched all matched products. Count: {productsView.Count}");
            ViewData["Query"] = query;
            return PartialView("_ProductList", productsView);
            }

        }
    }
