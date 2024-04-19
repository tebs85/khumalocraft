namespace KhumaloCraft.Services;

using KhumaloCraft.Models;
using System.Data.SqlClient;

public class AuthService
{
    private readonly IConfiguration _configuration;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        bool isAuthenticated = false;
        
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Username", username);
            command.Parameters.AddWithValue("@Password", password);
            
            try
            {
                await connection.OpenAsync();
                int result = (int)await command.ExecuteScalarAsync();
                isAuthenticated = result == 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        return isAuthenticated;
    }
}