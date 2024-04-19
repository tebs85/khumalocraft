namespace KhumaloCraft.Services;

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using KhumaloCraft.Models;

public interface IOrderService
{
    Task PlaceOrder(Order order, List<OrderDetails> orderDetails);
    Task<IEnumerable<Order>> GetOrderHistory(string UserID);
}

public class OrderService : IOrderService
{
    private readonly IConfiguration _configuration;

    public OrderService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task PlaceOrder(Order order, List<OrderDetails> orderDetails)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
        {
            await connection.OpenAsync();

            // Begin a transaction
            using (var connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
            {
                await connection.OpenAsync();

                // Begin a transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert order
                        var insertOrderQuery = "INSERT INTO Orders (UserID, OrderDate, TotalAmount) " +
                                            "VALUES (@UserID, @OrderDate, @TotalAmount);" +
                                            "SELECT SCOPE_IDENTITY();";

                        var command = new SqlCommand(insertOrderQuery, connection, transaction);
                        command.Parameters.AddWithValue("@UserID", order.UserID);
                        command.Parameters.AddWithValue("@OrderDate", order.OrderDate);
                        command.Parameters.AddWithValue("@TotalAmount", order.TotalAmount);

                        // Execute the command and retrieve the inserted order ID
                        var orderId = Convert.ToInt32(await command.ExecuteScalarAsync());

                        // Insert order details
                        foreach (var orderDetail in orderDetails)
                        {
                            var insertOrderDetailQuery = "INSERT INTO OrderDetails (OrderId, ProductID, Quantity, UnitPrice) " +
                                                        "VALUES (@OrderId, @ProductID, @Quantity, @UnitPrice);";

                            command.CommandText = insertOrderDetailQuery;
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@OrderId", orderId);
                            command.Parameters.AddWithValue("@ProductID", orderDetail.ProductID);
                            command.Parameters.AddWithValue("@Quantity", orderDetail.Quantity);
                            command.Parameters.AddWithValue("@UnitPrice", orderDetail.UnitPrice);

                            // Execute the command for each order detail
                            await command.ExecuteNonQueryAsync();
                        }

                        // Commit the transaction
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        // Rollback the transaction if an error occurs
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }

    public async Task<IEnumerable<Order>> GetOrderHistory(string userID)
    {
        var orders = new List<Order>();

        using (var connection = new SqlConnection(_configuration.GetConnectionString("KhumaloCraftConnectionString")))
        {
            await connection.OpenAsync();

            var query = "SELECT * FROM Orders WHERE UserID = @UserID ORDER BY OrderDate DESC;";
            orders = (await connection.QueryAsync<Order>(query, new { UserID = userID })).ToList();
        }

        return orders;
    }

}
