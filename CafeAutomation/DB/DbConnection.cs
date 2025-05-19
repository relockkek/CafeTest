using MySqlConnector;
using System.Data;
using System.Windows;

public class DbConnection : IDisposable
{
    private MySqlConnection _connection;

    public void Config()
    {
        var sb = new MySqlConnectionStringBuilder
        {
            Server = "95.154.107.102",
            UserID = "student",
            Password = "student",
            Database = "CafeAutomation",
            CharacterSet = "utf8mb4"
        };

        _connection = new MySqlConnection(sb.ConnectionString);
    }

    public bool OpenConnection()
    {
        if (_connection == null)
        {
            Config();
        }

        try
        {
            if (_connection.State != ConnectionState.Open)
            {
                _connection.Open();
            }

            return true;
        }
        catch (MySqlException e)
        {
            MessageBox.Show("Ошибка подключения к базе данных: " + e.Message);
            return false;
        }
    }

    public void CloseConnection()
    {
        if (_connection != null && _connection.State != ConnectionState.Closed)
        {
            try
            {
                _connection.Close();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Ошибка закрытия соединения: " + e.Message);
            }
        }
    }

    public MySqlCommand CreateCommand(string sql)
    {
        return new MySqlCommand(sql, _connection);
    }

    public static DbConnection GetDbConnection()
    {
        return new DbConnection();
    }

    private DbConnection()
    {
        Config();
    }
    public void Dispose()
    {
        CloseConnection();
    }
}