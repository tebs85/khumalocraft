namespace KhumaloCraft.Services;

using KhumaloCraft.Models;
using System.Data.SqlClient;

public class ProductService
{
    private readonly IConfiguration _configuration;

    public ProductService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IList<Product> GetProducts()
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
        {
            connection.Open();
            var commandText = "SELECT ProductID, ProductName, Description, Price, ImageURL, Availability FROM Products";
            using (var command = new SqlCommand(commandText, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    var products = new List<Product>();
                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Description = reader.GetString(2),
                            Price = reader.GetDecimal(3),
                            ImageURL = reader.GetString(4),
                            Availability = reader.GetBoolean(5)
                        });
                    }
                    return products;
                }
            }
        }
    }

    public void AddProduct(Product product)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
        {
            connection.Open();
            var commandText = "INSERT INTO Products (ProductName, Description, Price, ImageURL, CategoryID, Availability) VALUES (@ProductName, @Description, @Price, @ImageURL, @CategoryID, @Availability)";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@ProductName", product.ProductName);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@ImageURL", product.ImageURL);
                command.Parameters.AddWithValue("@CategoryID", product.CategoryID);
                command.Parameters.AddWithValue("@Availability", product.Availability);
                command.ExecuteNonQuery();
            }
        }
    }
}
