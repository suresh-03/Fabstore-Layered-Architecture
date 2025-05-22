using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Fabstore.Framework.CommonEnums;
namespace Fabstore.Domain.Models;


public class Payment
    {
    [Key]
    public int PaymentID { get; set; }

    // Foreign Key
    public int OrderID { get; set; }

    [ForeignKey("OrderID")]
    public Order? Order { get; set; }  // Navigation property

    public PaymentMethodType PaymentMethod { get; set; }  // e.g., UPI, Card, COD

    public DateTime PaymentDate { get; set; }

    [StringLength(100)]
    public string TransactionID { get; set; }
    }





