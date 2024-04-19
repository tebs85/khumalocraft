namespace KhumaloCraft.Services;

using System.Collections.Generic;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using KhumaloCraft.Models;

public class CategoryService
{
    private readonly string _connectionString;

    public CategoryService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("KhumaloCraftConnectionString");
    }

    public IList<Category> GetCategories()
    {
        IList<Category> categories = new List<Category>();
        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            string query = "SELECT CategoryID, CategoryName FROM Categories";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    categories.Add(new Category
                    {
                        CategoryID = reader.GetInt32(0),
                        CategoryName = reader.GetString(1)
                    });
                }
            }
        }
        return categories;
    }
}

