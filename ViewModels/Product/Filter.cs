namespace FabstoreWebAppliction.ViewModels.Product;

public class Filter
    {
    public string Category { get; set; } = string.Empty;
    public string SortRating { get; set; } = string.Empty;
    public string SortPrice { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public string Brand { get; set; } = string.Empty;

    public decimal MinPrice { get; set; } = 100;

    public decimal MaxPrice { get; set; } = 5000;
    }

