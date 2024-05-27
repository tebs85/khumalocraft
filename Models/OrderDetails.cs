using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models;

public class OrderDetails
{
    [Key]    
    public int OrderDetailID { get; set; }
    public int OrderID { get; set; }
    public int ProductID { get; set; }
    public int Quantity { get; set; }
    
    [Column(TypeName = "decimal(18, 2)")]
    public decimal UnitPrice { get; set; }
}
