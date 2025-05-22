using System.ComponentModel.DataAnnotations;


namespace FabstoreWebApplication.ViewModels;



public class ProductVariantView
    {

    public int VariantID { get; set; }

    // Foreign Key to Product
    public int ProductID { get; set; }


    public ProductView? Product { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }


    public decimal Price { get; set; }


    public decimal Discount { get; set; }

    public int Stock { get; set; }

    [Required]
    [StringLength(100)]
    public string SKU { get; set; }

    public ICollection<ProductImageView> Images { get; set; } = new List<ProductImageView>();
    public ICollection<CartView> Carts { get; set; } = new List<CartView>();
    public ICollection<WishlistView> Wishlists { get; set; } = new List<WishlistView>();

    public ICollection<OrderItemView> OrderItems { get; set; } = new List<OrderItemView>();

    }


