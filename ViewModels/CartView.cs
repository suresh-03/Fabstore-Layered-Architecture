using System.ComponentModel.DataAnnotations;

namespace FabstoreWebApplication.ViewModels;



public class CartView
    {

    public int CartID { get; set; }

    // Foreign Key to User
    [Required]
    public int UserID { get; set; }

    // Foreign Key to ProductVariant
    [Required]
    public int VariantID { get; set; }


    public UserView User { get; set; } = null!;


    public ProductVariantView Variant { get; set; } = null!;

    public bool IsDeleted { get; set; } = false;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; } = 1;

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }
    }


