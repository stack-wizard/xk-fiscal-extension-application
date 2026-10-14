namespace Mikos.XK.Fiscal.Model
{
    public class FiscalDatabaseSettings
    {
        public string Provider { get; set; } = "mssql";
        public string Username { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }

        public bool IsMySql()
        {
            return string.Equals(Provider, "mysql", System.StringComparison.OrdinalIgnoreCase);
        }

        public bool IsSqlServer()
        {
            return string.Equals(Provider, "mssql", System.StringComparison.OrdinalIgnoreCase)
                || string.Equals(Provider, "sqlserver", System.StringComparison.OrdinalIgnoreCase);
        }
    }
}
