using Fabstore.Domain.Interfaces.IProduct;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.ViewComponents
    {
    // ViewComponent for rendering the category menu in the UI
    public class CategoryMenuViewComponent : ViewComponent
        {
        // Service for product-related business logic
        private readonly IProductService _productService;
        // Logger for logging category menu events and errors
        private readonly ILogger<CategoryMenuViewComponent> _logger;

        // Constructor with dependency injection for logger and product service
        public CategoryMenuViewComponent(ILogger<CategoryMenuViewComponent> logger, IProductService productService)
            {
            _productService = productService;
            _logger = logger;
            }

        // Invoked when the view component is called in a view
        public async Task<IViewComponentResult> InvokeAsync()
            {
            try
                {
                // Fetch categories using the product service
                var serviceResponse = await _productService.GetCategoriesAsync();

                // Log a warning if the service response is not successful
                if (!serviceResponse.Success)
                    {
                    _logger.LogWarning(serviceResponse.Message);
                    }
                // Return the view with the category data
                return View(serviceResponse.Data);
                }
            catch (Exception ex)
                {
                // Log any exceptions that occur during category retrieval
                _logger.LogError(ex, "Exception occured when getting categories");
                }

            // Return the view with null data if an error occurs
            return View(null);
            }
        }
    }