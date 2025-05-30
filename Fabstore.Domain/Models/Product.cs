﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Fabstore.Framework.CommonEnums;
namespace Fabstore.Domain.Models;



public class Product
    {
    [Key]
    public int ProductID { get; set; }

    [Required]
    [StringLength(200)]
    public string ProductName { get; set; }

    // Foreign key to Brand
    public int BrandID { get; set; }

    [ForeignKey("BrandID")]
    public Brand? Brand { get; set; }

    // Foreign key to Category
    public int CategoryID { get; set; }

    [ForeignKey("CategoryID")]
    public Category? Category { get; set; }

    public string? Description { get; set; }

    public GenderType Gender { get; set; }  // Men, Women, Unisex

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();

    public ICollection<Review> Reviews { get; set; } = new List<Review>();


    }




