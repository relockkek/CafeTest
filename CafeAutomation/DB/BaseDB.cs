using MySqlConnector;
using CafeAutomation.DB;

internal class BaseDB
{
    internal DbConnection Connection { get; private set; }

    protected BaseDB()
    {
        Connection = DbConnection.GetDbConnection();
    }

    protected bool OpenConnection() => Connection.OpenConnection();

    protected MySqlCommand CreateCommand(string query) => Connection.CreateCommand(query);
}