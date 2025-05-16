using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

internal class TablesDB : BaseDB
{
    private static TablesDB instance;
    public static TablesDB GetDb() => instance ??= new TablesDB();

    private TablesDB() { }

    public bool Insert(Tables table)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "INSERT INTO Tables (TableNumber, Capacity, Zone, IsActive) VALUES (@number, @capacity, @zone, @active); SELECT LAST_INSERT_ID();";

            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@number", table.TableNumber);
                cmd.Parameters.AddWithValue("@capacity", table.Capacity);
                cmd.Parameters.AddWithValue("@zone", table.Zone);
                cmd.Parameters.AddWithValue("@active", table.IsActive);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        table.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении стола: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> InsertAsync(Tables table)
    {
        return await Task.Run(() => Insert(table));
    }

    public async Task<List<Tables>> SelectAllAsync()
    {
        List<Tables> list = new List<Tables>();
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return list;

            const string query = "SELECT ID, TableNumber, Capacity, Zone, IsActive FROM Tables";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Tables
                        {
                            ID = reader.GetInt32(0),
                            TableNumber = reader.GetInt32(1),
                            Capacity = reader.GetInt32(2),
                            Zone = reader.GetString(3),
                            IsActive = reader.GetBoolean(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки столов: " + ex.Message);
                }
            }
        }

        return list;
    }

    public async Task<bool> UpdateAsync(Tables table)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "UPDATE Tables SET TableNumber=@number, Capacity=@capacity, Zone=@zone, IsActive=@active WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@number", table.TableNumber);
                cmd.Parameters.AddWithValue("@capacity", table.Capacity);
                cmd.Parameters.AddWithValue("@zone", table.Zone);
                cmd.Parameters.AddWithValue("@active", table.IsActive);
                cmd.Parameters.AddWithValue("@id", table.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления стола: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Tables table)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "DELETE FROM Tables WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", table.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления стола: " + ex.Message);
                }
            }
        }

        return result;
    }
    public async Task<Tables?> GetByNumberAsync(int number)
    {
        Tables? table = null;

        using var db = DbConnection.GetDbConnection();
        if (!db.OpenConnection()) return null;

        string query = "SELECT ID, TableNumber, Capacity, Zone, IsActive FROM Tables WHERE TableNumber = @number";

        using var cmd = db.CreateCommand(query);
        cmd.Parameters.AddWithValue("@number", number);

        try
        {
            var reader = await Task.Run(() => cmd.ExecuteReader());
            if (reader.Read())
            {
                table = new Tables
                {
                    ID = reader.GetInt32(0),
                    TableNumber = reader.GetInt32(1),
                    Capacity = reader.GetInt32(2),
                    Zone = reader.GetString(3),
                    IsActive = reader.GetBoolean(4)
                };
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Ошибка при получении стола: " + ex.Message);
        }

        return table;
    }

}
