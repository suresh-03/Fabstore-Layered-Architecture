using System.ComponentModel.DataAnnotations;

namespace FabstoreWebApplication.ViewModels;



public class CategoryView
    {

    public int CategoryID { get; set; }

    [Required]
    [StringLength(100)]
    public string CategoryName { get; set; }

    public int? ParentCategoryID { get; set; }


    public CategoryView? ParentCategory { get; set; }

    public ICollection<CategoryView> SubCategories { get; set; } = new List<CategoryView>();

    public ICollection<ProductView> Products { get; set; } = new List<ProductView>();
    }


