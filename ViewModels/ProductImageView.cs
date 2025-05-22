using System.ComponentModel.DataAnnotations;
namespace FabstoreWebApplication.ViewModels;



public class ProductImageView
    {

    public int ImageID { get; set; }

    // Foreign Key to ProductVariant
    public int VariantID { get; set; }


    public ProductVariantView? Variant { get; set; }

    [StringLength(500)]
    public string? ImageUrl { get; set; }

    public bool IsPrimary { get; set; } = false;
    }
