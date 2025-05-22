namespace FabstoreWebApplication.ViewModels;

public class OrderItemView
    {

    public int OrderItemID { get; set; }


    public int OrderID { get; set; }


    public OrderView? Order { get; set; }

    public int VariantID { get; set; }

    public ProductVariantView? ProductVariant { get; set; }

    public int Quantity { get; set; }


    public decimal Price { get; set; }
    }
