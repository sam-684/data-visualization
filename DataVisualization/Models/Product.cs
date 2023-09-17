namespace DataVisualization.Models;

public class Product
{
    public string itemCode { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string manufacturer { get; set; } = string.Empty;
    public string? mpn { get; set; }
    public double price { get; set; }
    public string? brand { get; set; }
    public string? upc { get; set; }
}

public class ProductList
{
    public List<Product> products { get; set; } = new List<Product>();
}

