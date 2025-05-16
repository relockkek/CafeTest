using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using MySqlConnector;
using CafeAutomation.Models;

namespace CafeAutomation.DB
{
    internal class ReservationsDB : BaseDB
    {
        // Singleton
        private static ReservationsDB instance;
        public static ReservationsDB GetDb() => instance ??= new ReservationsDB();

        private ReservationsDB() { }

        public bool Insert(Reservations reservation)
        {
            bool result = false;
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return result;

                string query = "INSERT INTO Reservations (TableID, CustomerName, CustomerPhone, GuestsCount, ReservationDate, Status) VALUES (@tableId, @name, @phone, @guests, @date, @status); SELECT LAST_INSERT_ID();";

                using (var cmd = db.CreateCommand(query))
                {
                    cmd.Parameters.AddWithValue("@tableId", reservation.TableID);
                    cmd.Parameters.AddWithValue("@name", reservation.CustomerName);
                    cmd.Parameters.AddWithValue("@phone", reservation.CustomerPhone);
                    cmd.Parameters.AddWithValue("@guests", reservation.GuestsCount);
                    cmd.Parameters.AddWithValue("@date", reservation.ReservationDate);
                    cmd.Parameters.AddWithValue("@status", reservation.Status);

                    try
                    {
                        var id = cmd.ExecuteScalar();
                        if (id != null)
                        {
                            reservation.ID = Convert.ToInt32(id);
                            result = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при добавлении бронирования: " + ex.Message);
                    }
                }
            }

            return result;
        }

        public async Task<List<Reservations>> SelectAllAsync()
        {
            List<Reservations> list = new List<Reservations>();
            using (var db = DbConnection.GetDbConnection())
            {
                if (!db.OpenConnection()) return list;

                const string query = "SELECT ID, TableID, CustomerName, CustomerPhone, GuestsCount, ReservationDate, Status FROM Reservations";
                using (var cmd = db.CreateCommand(query))
                {
                    try
                    {
                        var reader = await Task.Run(() => cmd.ExecuteReader());

                        while (reader.Read())
                        {
                            list.Add(new Reservations
                            {
                                ID = reader.GetInt32(0),
                                TableID = reader.GetInt32(1),
                                CustomerName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                CustomerPhone = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                GuestsCount = reader.GetInt32(4),
                                ReservationDate = reader.GetDateTime(5),
                                Status = reader.IsDBNull(6) ? "" : reader.GetString(6)
                            });
                        }

                        reader.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка загрузки бронирований: " + ex.Message);
                    }
                }
            }

            return list;
        }

        public async Task<bool> UpdateAsync(Reservations reservation)
        {
            bool result = false;
            if (!OpenConnection()) return result;

            string query = "UPDATE Reservations SET TableID=@tableId, CustomerName=@name, CustomerPhone=@phone, GuestsCount=@guests, ReservationDate=@date, Status=@status WHERE ID=@id";
            using (var cmd = CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@tableId", reservation.TableID);
                cmd.Parameters.AddWithValue("@name", reservation.CustomerName);
                cmd.Parameters.AddWithValue("@phone", reservation.CustomerPhone);
                cmd.Parameters.AddWithValue("@guests", reservation.GuestsCount);
                cmd.Parameters.AddWithValue("@date", reservation.ReservationDate);
                cmd.Parameters.AddWithValue("@status", reservation.Status);
                cmd.Parameters.AddWithValue("@id", reservation.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка обновления бронирования: " + ex.Message);
                }
            }

            return result;
        }

        public async Task<bool> DeleteAsync(Reservations reservation)
        {
            bool result = false;
            if (!OpenConnection()) return result;

            string query = "DELETE FROM Reservations WHERE ID=@id";
            using (var cmd = CreateCommand(query))
            {
                cmd.Parameters.AddWithValue("@id", reservation.ID);

                try
                {
                    int rowsAffected = await Task.Run(() => cmd.ExecuteNonQuery());
                    result = rowsAffected > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка удаления бронирования: " + ex.Message);
                }
            }

            return result;
        }
    }
}