using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models;

public class Order
{
    public int OrderID { get; set; }
    public string UserID { get; set; }
    public DateTime OrderDate { get; set; }
        
    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

}
