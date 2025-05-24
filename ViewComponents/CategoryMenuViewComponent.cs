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

                // --------------- DEBUG ------------------- //

                //var categoriesDictionary = categoriesDb
                //    .AsParallel()
                //    .WithDegreeOfParallelism(4)
                //    .GroupBy(c =>
                //    {
                //        Console.WriteLine($"Grouping category '{c.CategoryName}' with ParentID {c.ParentCategoryID} on Thread {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.Name}");
                //        return c.ParentCategoryID;
                //    })
                //    .ToDictionary(
                //        g =>
                //        {
                //            Console.WriteLine($"Processing group key '{g.Key}' on Thread {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.Name}");
                //            return ((GenderType)g.Key).ToString();
                //        },
                //        g =>
                //        {
                //            var categoryNames = g.AsParallel().Select(c =>
                //            {
                //                Console.WriteLine($"Selecting '{c.CategoryName}' on Thread {Thread.CurrentThread.ManagedThreadId} {Thread.CurrentThread.Name}");
                //                return c.CategoryName;
                //            }).ToList();
                //            return categoryNames;
                //        }
                //    );

                // -----------------------------------------------------------------------//
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
