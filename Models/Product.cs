using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models;

public class Product
{
    public int ProductID { get; set; }
    public string ProductName { get; set; }
    public string Description { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    public bool Availability { get; set; }
    public int CategoryID { get; set; }
    public string ImageURL { get; set; }
}
