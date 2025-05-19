using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

namespace CafeAutomation.DB
{
    internal class EmployeesDB : BaseDB
    {
     
        private static EmployeesDB instance;
        public static EmployeesDB GetDb() => instance ??= new EmployeesDB();

        private EmployeesDB() { }

        public bool Insert(Employees employee)
        {
            bool result = false;
            if (!OpenConnection()) return result;

            string query = "INSERT INTO Employees (FirstName, LastName, Patronymic, Position, Phone, HireDate, Salary) VALUES (@fname, @lname, @patr, @pos, @phone, @hire, @salary); SELECT LAST_INSERT_ID();";

            using (var cmd = CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@fname", employee.FirstName);
                cmd.Parameters.AddWithValue("@lname", employee.LastName);
                cmd.Parameters.AddWithValue("@patr", employee.Patronymic);
                cmd.Parameters.AddWithValue("@pos", employee.Position);
                cmd.Parameters.AddWithValue("@phone", employee.Phone);
                cmd.Parameters.AddWithValue("@hire", employee.HireDate);
                cmd.Parameters.AddWithValue("@salary", employee.Salary);

                try
                {
                    var id = cmd.ExecuteScalar();
                    if (id != null)
                    {
                        employee.ID = Convert.ToInt32(id);
                        result = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении сотрудника: " + ex.Message);
                }
            }

            return result;
        }

        public async Task<List<Employees>> SelectAllAsync()
        {
            List<Employees> list = new List<Employees>();
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return list;

                const string query = "SELECT ID, FirstName, LastName, Patronymic, Position, Phone, HireDate, Salary FROM Employees";
                using (var cmd = db.CreateCommand(query))
                {
                    try
                    {
                        var reader = await Task.Run(() => cmd.ExecuteReader());

                        while (reader.Read())
                        {
                            list.Add(new Employees
                            {
                                ID = reader.GetInt32(0),
                                FirstName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                                LastName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Patronymic = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                Position = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                Phone = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                HireDate = reader.IsDBNull(6) ? DateTime.MinValue : reader.GetDateTime(6),
                                Salary = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7)
                            });
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки сотрудников: " + ex.Message);
                    }
                }
            }

            return list;
        }
        public async Task<bool> UpdateAsync(Employees employee)
        {
            bool result = false;
            if (!OpenConnection()) return result;

            string query = "UPDATE Employees SET FirstName=@fname, LastName=@lname, Patronymic=@patr, Position=@pos, Phone=@phone, HireDate=@hire, Salary=@salary WHERE ID=@id";
            using (var cmd = CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@fname", employee.FirstName);
                cmd.Parameters.AddWithValue("@lname", employee.LastName);
                cmd.Parameters.AddWithValue("@patr", employee.Patronymic);
                cmd.Parameters.AddWithValue("@pos", employee.Position);
                cmd.Parameters.AddWithValue("@phone", employee.Phone);
                cmd.Parameters.AddWithValue("@hire", employee.HireDate);
                cmd.Parameters.AddWithValue("@salary", employee.Salary);
                cmd.Parameters.AddWithValue("@id", employee.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления сотрудника: " + ex.Message);
                }
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Employees employee)
        {
            bool result = false;
            if (!OpenConnection()) return result;

            string query = "DELETE FROM Employees WHERE ID=@id";
            using (var cmd = CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", employee.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления сотрудника: " + ex.Message);
                }
            }

            return result;
        }
    }
}