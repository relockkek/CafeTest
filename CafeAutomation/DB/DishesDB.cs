// DishesDB.cs — полная версия с GetDb, Insert, UpdateAsync, DeleteAsync, SelectAllAsync и GetMostPopularDishAsync

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using CafeAutomation.Models;

namespace CafeAutomation.DB
{
    public class DishesDB
    {
        private static DishesDB instance;

        public static DishesDB GetDb()
        {
            if (instance == null)
                instance = new DishesDB();
            return instance;
        }

        public bool Insert(Dishes dish)
        {
            bool result = false;
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return result;

                string query = "INSERT INTO Dishes (Name, Price, Category, Description, IsAvailable, ImageData) " +
                               "VALUES (@name, @price, @category, @desc, @available, @image); SELECT LAST_INSERT_ID();";

                using (var cmd = db.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@name", dish.Name);
                    cmd.Parameters.AddWithValue("@price", dish.Price);
                    cmd.Parameters.AddWithValue("@category", dish.Category);
                    cmd.Parameters.AddWithValue("@desc", dish.Description);
                    cmd.Parameters.AddWithValue("@available", dish.IsAvailable);
                    cmd.Parameters.AddWithValue("@image", dish.ImageData ?? (object)DBNull.Value);

                    try
                    {
                        var id = cmd.ExecuteScalar();
                        if (id != null)
                        {
                            dish.ID = Convert.ToInt32(id);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при добавлении блюда: " + ex.Message);
                    }
                }
            }

            return result;
        }

        public async Task<bool> UpdateAsync(Dishes dish)
        {
            bool result = false;
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return result;

                string query = "UPDATE Dishes SET Name=@name, Price=@price, Category=@category, Description=@desc, IsAvailable=@available, ImageData=@image WHERE ID=@id";

                using (var cmd = db.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@name", dish.Name);
                    cmd.Parameters.AddWithValue("@price", dish.Price);
                    cmd.Parameters.AddWithValue("@category", dish.Category);
                    cmd.Parameters.AddWithValue("@desc", dish.Description);
                    cmd.Parameters.AddWithValue("@available", dish.IsAvailable);
                    cmd.Parameters.AddWithValue("@image", dish.ImageData ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@id", dish.ID);

                    try
                    {
                        int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                        result = rowsAffected > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка обновления блюда: " + ex.Message);
                    }
                }
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Dishes dish)
        {
            bool result = false;
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return result;

                string query = "DELETE FROM Dishes WHERE ID=@id";
                using (var cmd = db.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@id", dish.ID);

                    try
                    {
                        int rows = await Task.Run(() => cmd.ExecuteNonQuery());
                        result = rows > 0;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка удаления блюда: " + ex.Message);
                    }
                }
            }

            return result;
        }

        public async Task<List<Dishes>> SelectAllAsync()
        {
            List<Dishes> list = new List<Dishes>();
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return list;

                const string query = "SELECT ID, Name, Price, Category, Description, IsAvailable, ImageData FROM Dishes";
                using (var cmd = db.CreateCommand(query))
                {
                    try
                    {
                        var reader = await Task.Run(() => cmd.ExecuteReader());

                        while (reader.Read())
                        {
                            list.Add(new Dishes
                            {
                                ID = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Price = reader.GetDecimal(2),
                                Category = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                IsAvailable = reader.GetBoolean(5),
                                ImageData = reader.IsDBNull(6) ? null : (byte[])reader["ImageData"]
                            });
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки блюд: " + ex.Message);
                    }
                }
            }

            return list;
        }

        public async Task<Dishes> GetMostPopularDishAsync()
        {
            using var db = DbConnection.GetDbConnection();
            if (!db.OpenConnection()) return null;

            string query = @"
                SELECT d.ID, d.Name, d.Price, d.Category, d.Description, d.IsAvailable, d.ImageData
                FROM OrderItems oi
                JOIN Dishes d ON oi.DishID = d.ID
                GROUP BY oi.DishID
                ORDER BY COUNT(*) DESC
                LIMIT 1;
            ";

            using var cmd = db.CreateCommand(query);
            try
            {
                var reader = await Task.Run(() => cmd.ExecuteReader());
                if (reader.Read())
                {
                    return new Dishes
                    {
                        ID = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Price = reader.GetDecimal(2),
                        Category = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        Description = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        IsAvailable = reader.GetBoolean(5),
                        ImageData = reader.IsDBNull(6) ? null : (byte[])reader["ImageData"]
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении популярного блюда: " + ex.Message);
            }

            return null;
        }
        public async Task<bool> InsertAsync(Dishes dish)
        {
            return await Task.Run(() => Insert(dish));
        }

    }
}
