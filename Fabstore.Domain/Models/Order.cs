﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Fabstore.Framework.CommonEnums;
namespace Fabstore.Domain.Models;


public class Order
    {
    [Key]
    public int OrderID { get; set; }

    // Foreign Key
    public int UserID { get; set; }

    [ForeignKey("UserID")]
    public User? User { get; set; }  // Navigation property

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public OrderStatusType Status { get; set; }  // e.g., Placed, Shipped, Delivered, Cancelled

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalAmount { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal DiscountAmount { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal FinalAmount { get; set; }

    [StringLength(50)]
    public PaymentStatusType PaymentStatus { get; set; }  // Pending, Completed, Failed

    public string ShippingAddress { get; set; }

    // Navigation property - one order can have many order items
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public ICollection<Payment> Payments { get; set; } = new List<Payment>();  // Navigation property for payments
    }





