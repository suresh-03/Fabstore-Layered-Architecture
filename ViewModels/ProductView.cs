using System.ComponentModel.DataAnnotations;
using static Fabstore.Framework.CommonEnums;
namespace FabstoreWebApplication.ViewModels;



public class ProductView
    {

    public int ProductID { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; }


    public int BrandID { get; set; }


    public BrandView? Brand { get; set; }


    public int CategoryID { get; set; }

    public CategoryView? Category { get; set; }

    public string? Description { get; set; }

    public GenderType Gender { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductVariantView> Variants { get; set; } = new List<ProductVariantView>();

    public ICollection<ReviewView> Reviews { get; set; } = new List<ReviewView>();


    }




