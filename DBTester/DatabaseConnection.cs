using Npgsql;

namespace DBTester
{
    public class DatabaseConnection
    {
        public string User { get; set; }

        public string Password { get; set; }

        public string Host { get; set; }

        public int Port { get; set; }

        public string Database { get; set; }

        public string GetConnectionString()
        {
            return $"User ID={User};Password={Password};Host={Host};Port={Port};Database={Database};";
        }

        public NpgsqlConnection GetConnection()
        {
            var connectionString = GetConnectionString();
            return new NpgsqlConnection(connectionString);
        }
    }
}