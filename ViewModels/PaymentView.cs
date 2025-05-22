using System.ComponentModel.DataAnnotations;
namespace FabstoreWebApplication.ViewModels;
using static Fabstore.Framework.CommonEnums;



public class PaymentView
    {

    public int PaymentID { get; set; }

    // Foreign Key
    public int OrderID { get; set; }

    public OrderView? Order { get; set; }  // Navigation property

    public PaymentMethodType PaymentMethod { get; set; }  // e.g., UPI, Card, COD

    public DateTime PaymentDate { get; set; }

    [StringLength(100)]
    public string TransactionID { get; set; }
    }

