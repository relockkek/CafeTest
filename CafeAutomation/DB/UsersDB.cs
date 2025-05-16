using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

internal class UsersDB : BaseDB
{
    private static UsersDB instance;
    public static UsersDB GetDb() => instance ??= new UsersDB();

    private UsersDB() { }

    public bool Insert(Users user)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "INSERT INTO Users (EmployeeID, Username, Password, Role) VALUES (@empId, @username, @password, @role); SELECT LAST_INSERT_ID();";

            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@empId", user.EmployeeID);
                cmd.Parameters.AddWithValue("@username", user.Username);
                cmd.Parameters.AddWithValue("@password", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        user.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении пользователя: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<List<Users>> SelectAllAsync()
    {
        List<Users> list = new List<Users>();
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return list;

            const string query = "SELECT ID, EmployeeID, Username, Password, Role FROM Users";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var reader = await Task.Run(() => cmd.ExecuteReader());

                    while (reader.Read())
                    {
                        list.Add(new Users
                        {
                            ID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4)
                        });
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка загрузки пользователей: " + ex.Message);
                }
            }
        }

        return list;
    }

    public async Task<bool> UpdateAsync(Users user)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "UPDATE Users SET EmployeeID=@empId, Username=@user, Password=@pass, Role=@role WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@empId", user.EmployeeID);
                cmd.Parameters.AddWithValue("@user", user.Username);
                cmd.Parameters.AddWithValue("@pass", user.Password);
                cmd.Parameters.AddWithValue("@role", user.Role);
                cmd.Parameters.AddWithValue("@id", user.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления пользователя: " + ex.Message);
                }
            }
        }

        return result;
    }

    public async Task<bool> DeleteAsync(Users user)
    {
        bool result = false;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return result;

            string query = "DELETE FROM Users WHERE ID=@id";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", user.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления пользователя: " + ex.Message);
                }
            }
        }

        return result;
    }

    public Users FindByUsernameAndPassword(string username, string password)
    {
        Users user = null;
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return user;

            const string query = "SELECT ID, EmployeeID, Username, Password, Role FROM Users WHERE Username=@username AND Password=@password";
            using (var cmd = db.CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@password", password);

                try
                {
                    var reader = Task.Run(() => cmd.ExecuteReader()).Result;

                    if (reader.Read())
                    {
                        user = new Users
                        {
                            ID = reader.GetInt32(0),
                            EmployeeID = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            Password = reader.GetString(3),
                            Role = reader.GetString(4)
                        };
                    }

                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при поиске пользователя: " + ex.Message);
                }
            }
        }

        return user;
    }

    public bool CheckAdminExists()
    {
        using (var db = DbConnection.GetDbConnection())
        {
            if (!db.OpenConnection()) return false;

            const string query = "SELECT COUNT(*) FROM Users WHERE Username = 'admin'";
            using (var cmd = db.CreateCommand(query))
            {
                try
                {
                    var count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка проверки наличия администратора: " + ex.Message);
                }
            }
        }

        return false;
    }
}