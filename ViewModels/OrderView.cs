using System.ComponentModel.DataAnnotations;
namespace FabstoreWebApplication.ViewModels;
using static Fabstore.Framework.CommonEnums;



public class OrderView
    {

    public int OrderID { get; set; }


    public int UserID { get; set; }


    public UserView? User { get; set; }  // Navigation property

    public DateTime OrderDate { get; set; } = DateTime.Now;

    public OrderStatusType Status { get; set; }  // e.g., Placed, Shipped, Delivered, Cancelled

    public decimal TotalAmount { get; set; }

    public decimal DiscountAmount { get; set; }

    public decimal FinalAmount { get; set; }

    [StringLength(50)]
    public PaymentStatusType PaymentStatus { get; set; }  // Pending, Completed, Failed

    public string ShippingAddress { get; set; }

    // Navigation property - one order can have many order items
    public ICollection<OrderItemView> OrderItems { get; set; } = new List<OrderItemView>();

    public ICollection<PaymentView> Payments { get; set; } = new List<PaymentView>();  // Navigation property for payments
    }





