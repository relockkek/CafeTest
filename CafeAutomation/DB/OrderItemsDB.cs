using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

internal class OrderItemsDB : BaseDB
{
    private static OrderItemsDB instance;
    public static OrderItemsDB GetDb() => instance ??= new OrderItemsDB();

    private OrderItemsDB() { }

    public bool Insert(OrderItems item)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "INSERT INTO OrderItems (OrderID, DishID, Amount, PriceAtOrderTime) VALUES (@orderId, @dishId, @amount, @price); SELECT LAST_INSERT_ID();";

            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@orderId", item.OrderID);
                cmd.Parameters.AddWithValue("@dishId", item.DishID);
                cmd.Parameters.AddWithValue("@amount", item.Amount);
                cmd.Parameters.AddWithValue("@price", item.PriceAtOrderTime);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        item.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении элемента заказа: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> InsertAsync(OrderItems item)
    {
        return await Task.Run(() => Insert(item));
    }

    public async Task<List<OrderItems>> SelectAllAsync()
    {
        List<OrderItems> list = new List<OrderItems>();
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return list;

            const string query = "SELECT ID, OrderID, DishID, Amount, PriceAtOrderTime FROM OrderItems";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new OrderItems
                        {
                            ID = reader.GetInt32(0),
                            OrderID = reader.GetInt32(1),
                            DishID = reader.GetInt32(2),
                            Amount = reader.GetInt32(3),
                            PriceAtOrderTime = reader.GetDecimal(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки элементов заказа: " + ex.Message);
                }
            }
        }

        return list;
    }

    public async Task<bool> UpdateAsync(OrderItems item)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "UPDATE OrderItems SET OrderID=@orderId, DishID=@dishId, Amount=@amount, PriceAtOrderTime=@price WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@orderId", item.OrderID);
                cmd.Parameters.AddWithValue("@dishId", item.DishID);
                cmd.Parameters.AddWithValue("@amount", item.Amount);
                cmd.Parameters.AddWithValue("@price", item.PriceAtOrderTime);
                cmd.Parameters.AddWithValue("@id", item.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления элемента заказа: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<List<(string Name, int Quantity, decimal Total)>> GetOrderDetailsAsync(int orderId)
    {
        List<(string Name, int Quantity, decimal Total)> items = new();
        using var db = DbConnection.GetDbConnection();
        if (!db.OpenConnection()) return items;

        string query = @"
            SELECT d.Name, oi.Amount, oi.PriceAtOrderTime
            FROM OrderItems oi
            JOIN Dishes d ON d.ID = oi.DishID
            WHERE oi.OrderID = @orderId;
        ";

        using var cmd = db.CreateCommand(query);
        cmd.Parameters.AddWithValue("@orderId", orderId);

        try
        {
            using var reader = await Task.Run(() => cmd.ExecuteReader());
            while (reader.Read())
            {
                string name = reader.GetString(0);
                int qty = reader.GetInt32(1);
                decimal price = reader.GetDecimal(2);
                items.Add((name, qty, qty * price));
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка загрузки состава заказа: " + ex.Message);
        }

        return items;
    }

    public async Task<bool> DeleteAsync(OrderItems item)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "DELETE FROM OrderItems WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", item.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления элемента заказа: " + ex.Message);
                }
            }
        }

        return result;
    }
}
