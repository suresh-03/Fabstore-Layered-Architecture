using Fabstore.Domain.Interfaces.IProduct;
using Microsoft.AspNetCore.Mvc;

namespace FabstoreWebApplication.ViewComponents
    {
    public class CategoryMenuViewComponent : ViewComponent
        {
        private readonly IProductService _productService;
        private readonly ILogger<CategoryMenuViewComponent> _logger;
        public CategoryMenuViewComponent(ILogger<CategoryMenuViewComponent> logger, IProductService productService)
            {
            _productService = productService;
            _logger = logger;
            }


        public async Task<IViewComponentResult> InvokeAsync()
            {
            try
                {
                var serviceResponse = await _productService.GetCategoriesAsync();

                if (!serviceResponse.Success)
                    {
                    _logger.LogWarning(serviceResponse.Message);
                    }
                return View(serviceResponse.Data);
                }
            catch (Exception ex)
                {
                _logger.LogError(ex, "Exception occured when getting categories");
                }

            return View(null);

            }


        }
    }
