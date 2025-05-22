namespace FabstoreWebApplication.ViewModels;


public class WishlistView
    {

    public int WishlistID { get; set; }


    public int UserID { get; set; }


    public UserView? User { get; set; }


    public int VariantID { get; set; }


    public ProductVariantView? Variant { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.Now;
    }


