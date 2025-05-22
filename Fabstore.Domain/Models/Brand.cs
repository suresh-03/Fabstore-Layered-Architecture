﻿using System.ComponentModel.DataAnnotations;

namespace Fabstore.Domain.Models;



public class Brand
    {
    [Key]
    public int BrandID { get; set; }

    [Required]
    [StringLength(100)]
    public string BrandName { get; set; }

    // Navigation property
    public ICollection<Product> Products { get; set; } = new List<Product>();
    }


