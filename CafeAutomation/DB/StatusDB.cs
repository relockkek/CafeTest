using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

internal class StatusDB : BaseDB
{
    private static StatusDB instance;
    public static StatusDB GetDb() => instance ??= new StatusDB();

    private StatusDB() { }

    public bool Insert(Status status)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "INSERT INTO Status (Title) VALUES (@title); SELECT LAST_INSERT_ID();";

            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@title", status.Title);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        status.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении статуса: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<List<Status>> SelectAllAsync()
    {
        List<Status> list = new List<Status>();
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return list;

            const string query = "SELECT ID, Title FROM Status";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Status
                        {
                            ID = reader.GetInt32(0),
                            Title = reader.GetString(1)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки статусов: " + ex.Message);
                }
            }
        }

        return list;
    }

    public async Task<bool> UpdateAsync(Status status)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "UPDATE Status SET Title=@title WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@title", status.Title);
                cmd.Parameters.AddWithValue("@id", status.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления статуса: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Status status)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "DELETE FROM Status WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", status.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления статуса: " + ex.Message);
                }
            }
        }

        return result;
    }
}