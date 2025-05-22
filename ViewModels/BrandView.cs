namespace FabstoreWebApplication.ViewModels;



public class BrandView
    {

    public int BrandID { get; set; }


    public string BrandName { get; set; }

    // Navigation property
    public ICollection<ProductView> Products { get; set; } = new List<ProductView>();
    }


