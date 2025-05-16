using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

internal class OrdersDB : BaseDB
{
    private static OrdersDB instance;
    public static OrdersDB GetDb() => instance ??= new OrdersDB();

    private OrdersDB() { }

    public bool Insert(Orders order)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "INSERT INTO Orders (EmployeeID, TableNumber, OrderDate, TotalAmount, StatusID) VALUES (@empId, @table, @date, @total, @statusId); SELECT LAST_INSERT_ID();";

            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@empId", order.EmployeeID);
                cmd.Parameters.AddWithValue("@table", order.TableNumber);
                cmd.Parameters.AddWithValue("@date", order.OrderDate);
                cmd.Parameters.AddWithValue("@total", order.TotalAmount);
                cmd.Parameters.AddWithValue("@statusId", order.StatusID);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        order.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка добавления заказа: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<List<Orders>> SelectAllAsync()
    {
        List<Orders> list = new List<Orders>();
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return list;

            const string query = "SELECT ID, EmployeeID, TableNumber, OrderDate, TotalAmount, StatusID FROM Orders";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Orders
                        {
                            ID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            TableNumber = reader.GetInt32(2),
                            OrderDate = reader.GetDateTime(3),
                            TotalAmount = reader.GetDecimal(4),
                            StatusID = reader.GetInt32(5)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки заказов: " + ex.Message);
                }
            }
        }

        return list;
    }

    public async Task<bool> UpdateAsync(Orders order)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "UPDATE Orders SET EmployeeID=@empId, TableNumber=@table, OrderDate=@date, TotalAmount=@total, StatusID=@statusId WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@empId", order.EmployeeID);
                cmd.Parameters.AddWithValue("@table", order.TableNumber);
                cmd.Parameters.AddWithValue("@date", order.OrderDate);
                cmd.Parameters.AddWithValue("@total", order.TotalAmount);
                cmd.Parameters.AddWithValue("@statusId", order.StatusID);
                cmd.Parameters.AddWithValue("@id", order.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления заказа: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Orders order)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "DELETE FROM Orders WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", order.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления заказа: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<decimal> GetTotalRevenueAsync(DateTime start, DateTime end)
    {
        decimal totalRevenue = 0;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return totalRevenue;

            string query = "SELECT SUM(TotalAmount) FROM Orders WHERE OrderDate BETWEEN @start AND @end";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                try
                {
                    var res = await Task.Run(() => cmd.ExecuteScalar());
                    if (res != null && res != DBNull.Value)
                        totalRevenue = Convert.ToDecimal(res);
                }
                catch { }
            }
        }

        return totalRevenue;
    }

    public async Task<int> GetOrdersCountAsync(DateTime start, DateTime end)
    {
        int ordersCount = 0;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return ordersCount;

            string query = "SELECT COUNT(*) FROM Orders WHERE OrderDate BETWEEN @start AND @end";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@start", start);
                cmd.Parameters.AddWithValue("@end", end);

                try
                {
                    var res = await Task.Run(() => cmd.ExecuteScalar());
                    if (res != null && res != DBNull.Value)
                        ordersCount = Convert.ToInt32(res);
                }
                catch { }
            }
        }

        return ordersCount;
    }
}