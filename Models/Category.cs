using System.ComponentModel.DataAnnotations.Schema;

namespace KhumaloCraft.Models;

[Table("Categories")]
public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; }
}
