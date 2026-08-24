using Mikos.XK.Fiscal.Model;
using MySql.Data.MySqlClient;
using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;

namespace Mikos.XK.Fiscal.Datastore.Dao
{
    internal static class FiscalConnectionFactory
    {
        private const string FiscalDatabaseName = "mikos_fiscal_xk";
        private const int DefaultMySqlPort = 3306;

        private static readonly object SyncRoot = new object();
        private static FiscalDatabaseSettings _settings = new FiscalDatabaseSettings();

        public static void Configure(FiscalDatabaseSettings settings)
        {
            lock (SyncRoot)
            {
                _settings = settings ?? new FiscalDatabaseSettings();
            }
        }

        public static DbConnection CreateFiscalConnection() => CreateConnection(FiscalDatabaseName);

        private static DbConnection CreateConnection(string databaseName)
        {
            FiscalDatabaseSettings settings = GetSettings();
            if (settings.IsMySql())
            {
                return CreateMySqlConnection(settings, databaseName);
            }
            if (settings.IsSqlServer())
            {
                return CreateSqlServerConnection(settings, databaseName);
            }
            throw new InvalidOperationException($"Unsupported fiscal database provider '{settings.Provider}'. Use 'mysql' or 'mssql'.");
        }

        private static FiscalDatabaseSettings GetSettings()
        {
            lock (SyncRoot)
            {
                return _settings ?? new FiscalDatabaseSettings();
            }
        }

        private static DbConnection CreateMySqlConnection(FiscalDatabaseSettings settings, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(settings.Username) || string.IsNullOrWhiteSpace(settings.Password))
            {
                throw new InvalidOperationException("FiscalConfig database provider is mysql but username/password are missing.");
            }

            var builder = new MySqlConnectionStringBuilder
            {
                Server = string.IsNullOrWhiteSpace(settings.Host) ? "localhost" : settings.Host,
                Port = (uint)(settings.Port.GetValueOrDefault(DefaultMySqlPort)),
                Database = databaseName,
                UserID = settings.Username,
                Password = settings.Password,
                SslMode = MySqlSslMode.Disabled,
                AllowPublicKeyRetrieval = true
            };

            return new MySqlConnection(builder.ConnectionString);
        }

        private static DbConnection CreateSqlServerConnection(FiscalDatabaseSettings settings, string databaseName)
        {
            if (string.IsNullOrWhiteSpace(settings.Host))
            {
                return new SqlConnectionFactory().CreateConnection(databaseName) as DbConnection;
            }

            var builder = new SqlConnectionStringBuilder
            {
                DataSource = BuildSqlServerDataSource(settings.Host, settings.Port),
                InitialCatalog = databaseName
            };

            if (string.IsNullOrWhiteSpace(settings.Username) || string.IsNullOrWhiteSpace(settings.Password))
            {
                builder.IntegratedSecurity = true;
            }
            else
            {
                builder.UserID = settings.Username;
                builder.Password = settings.Password;
                builder.IntegratedSecurity = false;
            }

            return new SqlConnection(builder.ConnectionString);
        }

        private static string BuildSqlServerDataSource(string host, int? port)
        {
            if (!port.HasValue || port.Value <= 0)
            {
                return host;
            }

            return $"{host},{port.Value}";
        }
    }
}
