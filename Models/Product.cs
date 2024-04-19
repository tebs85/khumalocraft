namespace KhumaloCraft.Models;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public bool Availability { get; set; }
    public int CategoryID { get; set; }
    public string ImageURL { get; set; }
}
