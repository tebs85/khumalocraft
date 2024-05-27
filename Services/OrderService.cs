using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KhumaloCraft.Models;
using Microsoft.Extensions.Configuration;

namespace KhumaloCraft.Services
{
    public interface IOrderService
    {
        Task PlaceOrder(Order order, List<OrderDetails> orderDetails);
        Task<IEnumerable<Order>> GetOrderHistory(string userId);
    }

    public class OrderService : IOrderService
    {
        private readonly string _connectionString;

        public OrderService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("KhumaloCraftConnectionString");
        }

        public async Task PlaceOrder(Order order, List<OrderDetails> orderDetails)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                // Begin a transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Insert order
                        var insertOrderQuery = "INSERT INTO Orders (UserId, OrderDate, TotalAmount) " +
                                               "VALUES (@UserId, @OrderDate, @TotalAmount);" +
                                               "SELECT SCOPE_IDENTITY();";

                        var orderId = await ExecuteScalarAsync(connection, insertOrderQuery, new
                        {
                            UserId = order.UserID,
                            OrderDate = order.OrderDate,
                            TotalAmount = order.TotalAmount
                        }, transaction);

                        // Insert order details
                        foreach (var orderDetail in orderDetails)
                        {
                            var insertOrderDetailQuery = "INSERT INTO OrderDetails (OrderId, ProductId, Quantity, UnitPrice) " +
                                                         "VALUES (@OrderId, @ProductId, @Quantity, @UnitPrice);";

                            await ExecuteAsync(connection, insertOrderDetailQuery, new
                            {
                                OrderId = orderId,
                                ProductId = orderDetail.ProductID,
                                Quantity = orderDetail.Quantity,
                                UnitPrice = orderDetail.UnitPrice
                            }, transaction);
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

        public async Task<IEnumerable<Order>> GetOrderHistory(string userId)
        {
            var orders = new List<Order>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM Orders WHERE UserId = @UserId ORDER BY OrderDate DESC;";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderID = reader.GetInt32(reader.GetOrdinal("OrderID")),
                                UserID = reader.GetString(reader.GetOrdinal("UserId")),
                                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                                TotalAmount = reader.GetDecimal(reader.GetOrdinal("TotalAmount"))
                            };
                            orders.Add(order);
                        }
                    }
                }
            }

            return orders;
        }

        private async Task<object> ExecuteScalarAsync(SqlConnection connection, string query, object parameters, SqlTransaction transaction = null)
        {
            using (var command = new SqlCommand(query, connection))
            {
                if (transaction != null)
                    command.Transaction = transaction;
                AddParameters(command, parameters);
                return await command.ExecuteScalarAsync();
            }
        }

        private async Task ExecuteAsync(SqlConnection connection, string query, object parameters, SqlTransaction transaction = null)
        {
            using (var command = new SqlCommand(query, connection))
            {
                if (transaction != null)
                    command.Transaction = transaction;
                AddParameters(command, parameters);
                await command.ExecuteNonQueryAsync();
            }
        }

        private void AddParameters(SqlCommand command, object parameters)
        {
            if (parameters != null)
            {
                foreach (var prop in parameters.GetType().GetProperties())
                {
                    command.Parameters.AddWithValue($"@{prop.Name}", prop.GetValue(parameters));
                }
            }
        }
    }
}
