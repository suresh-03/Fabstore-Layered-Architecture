﻿
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fabstore.Domain.Models;


public class ProductVariant
    {
    [Key]
    public int VariantID { get; set; }

    // Foreign Key to Product
    public int ProductID { get; set; }

    [ForeignKey("ProductID")]
    public Product? Product { get; set; }

    [StringLength(20)]
    public string? Size { get; set; }

    [StringLength(50)]
    public string? Color { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal Price { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal Discount { get; set; }

    public int Stock { get; set; }

    [Required]
    [StringLength(100)]
    public string SKU { get; set; }

    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public ICollection<Wishlist> Wishlists { get; set; } = new List<Wishlist>();

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }


