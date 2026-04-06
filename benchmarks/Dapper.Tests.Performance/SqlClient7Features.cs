using System;
using System.Data;
using Microsoft.Data.SqlClient;
using BenchmarkDotNet.Attributes;

namespace Dapper.Tests.Performance
{
    /// <summary>
    /// Examples demonstrating Microsoft.Data.SqlClient 7.0 features
    /// These are not benchmarks, but educational examples
    /// </summary>
    public static class SqlClient7Examples
    {
        /// <summary>
        /// Example 1: Using Configurable Retry Logic (v7.0 feature)
        /// Provides resilience against transient failures
        /// </summary>
        public static void ConfigurableRetryLogicExample()
        {
            var connectionString = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True;TrustServerCertificate=True";

            // Create retry logic options
            var retryOption = new SqlRetryLogicOption
            {
                NumberOfTries = 3,
                DeltaTime = TimeSpan.FromSeconds(1),
                MaxTimeInterval = TimeSpan.FromSeconds(20),
                TransientErrors = SqlConfigurableRetryFactory.BaselineTransientErrors // v7.0 exposes this list
            };

            // Create retry logic provider with exponential backoff
            var retryLogicProvider = SqlConfigurableRetryFactory.CreateExponentialRetryProvider(retryOption);

            using var connection = new SqlConnection(connectionString);
            connection.RetryLogicProvider = retryLogicProvider;

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            command.RetryLogicProvider = retryLogicProvider; // Can also be set on command level

            connection.Open();
            var result = command.ExecuteScalar();

            Console.WriteLine($"Query executed with retry logic protection. Result: {result}");
        }

        /// <summary>
        /// Example 2: Using Enhanced Connection String Builder (v7.0 optimized)
        /// </summary>
        public static string BuildOptimizedConnectionString()
        {
            var builder = new SqlConnectionStringBuilder
            {
                DataSource = ".",
                InitialCatalog = "tempdb",
                IntegratedSecurity = true,
                TrustServerCertificate = true,

                // Connection Resiliency (v7.0 improved)
                ConnectRetryCount = 1,
                ConnectRetryInterval = 10,

                // Connection Pooling (v7.0 performance improvements)
                Pooling = true,
                MaxPoolSize = 100,
                MinPoolSize = 5,

                // Connection Timeout
                ConnectTimeout = 15,

                // Application Intent (v7.0 enhanced routing for read replicas)
                ApplicationIntent = ApplicationIntent.ReadWrite,

                // Multi-Subnet Failover (v7.0 enhanced failover support)
                // Alternatively, use app context switch to enable globally:
                // AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault", true);
                MultiSubnetFailover = false
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Example 3: Leveraging Baseline Transient Errors (v7.0 feature)
        /// The baseline transient error list is now publicly exposed
        /// </summary>
        public static void PrintBaselineTransientErrors()
        {
            Console.WriteLine("Microsoft.Data.SqlClient 7.0 Baseline Transient Errors:");
            Console.WriteLine("These errors are automatically retried by the configurable retry logic:");
            Console.WriteLine();

            foreach (var errorCode in SqlConfigurableRetryFactory.BaselineTransientErrors)
            {
                Console.WriteLine($"  - Error Code: {errorCode}");
            }
        }

        /// <summary>
        /// Example 4: Using App Context Switches (v7.0 feature)
        /// These can be set programmatically or in app.config
        /// </summary>
        public static void ConfigureAppContextSwitches()
        {
            // Enable Multi-Subnet Failover by default (v7.0+)
            // Useful for Always On Availability Groups
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault", true);

            // Enable User Agent feature (v7.0+)
            // Provides better telemetry and diagnostics
            AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableUserAgent", true);

            // Ignore server-provided failover partner (v7.0+)
            // Useful for Basic Availability Groups
            // AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.IgnoreServerProvidedFailoverPartner", true);

            Console.WriteLine("App context switches configured for Microsoft.Data.SqlClient 7.0");
        }

#if MSSQLCLIENT
        /// <summary>
        /// Example 5: Using Diagnostic Events (v7.0 brings these to .NET Framework)
        /// Previously only available on .NET Core, now on all platforms
        /// </summary>
        public static IDisposable EnableDiagnosticLogging()
        {
            return SqlClientDiagnostics.EnableDiagnostics(
                onCommand: (eventName, eventValue) =>
                {
                    Console.WriteLine($"[Command Event] {eventName}");
                },
                onConnection: (eventName, eventValue) =>
                {
                    Console.WriteLine($"[Connection Event] {eventName}");
                },
                onTransaction: (eventName, eventValue) =>
                {
                    Console.WriteLine($"[Transaction Event] {eventName}");
                }
            );
        }

        /// <summary>
        /// Example 6: Using the Diagnostic Logger
        /// </summary>
        public static void UseDiagnosticLogger()
        {
            var logger = new SqlDiagnosticLogger(msg => Console.WriteLine(msg));

            using var subscription = SqlClientDiagnostics.EnableDiagnostics(logger);
            using var connection = new SqlConnection(BenchmarkBase.ConnectionString);

            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT GETDATE()";
            var result = command.ExecuteScalar();

            Console.WriteLine($"Current server time: {result}");
            // Diagnostic events will be logged automatically
        }
#endif

        /// <summary>
        /// Example 7: SqlBulkCopy with Hidden Columns (v7.0 feature)
        /// SqlBulkCopy can now operate on hidden columns (e.g., temporal tables)
        /// No code changes required - this works automatically
        /// </summary>
        public static void SqlBulkCopyWithHiddenColumns()
        {
            var connectionString = "Data Source=.;Initial Catalog=tempdb;Integrated Security=True;TrustServerCertificate=True";

            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // Create a temporal table (has hidden columns for system versioning)
            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    IF OBJECT_ID('dbo.TemporalExample', 'U') IS NOT NULL
                        DROP TABLE dbo.TemporalExample;

                    CREATE TABLE dbo.TemporalExample
                    (
                        Id INT PRIMARY KEY,
                        Name NVARCHAR(100),
                        -- v7.0 SqlBulkCopy can now handle these hidden period columns automatically
                        ValidFrom DATETIME2 GENERATED ALWAYS AS ROW START HIDDEN,
                        ValidTo DATETIME2 GENERATED ALWAYS AS ROW END HIDDEN,
                        PERIOD FOR SYSTEM_TIME (ValidFrom, ValidTo)
                    )
                    WITH (SYSTEM_VERSIONING = ON (HISTORY_TABLE = dbo.TemporalExampleHistory));
                ";
                command.ExecuteNonQuery();
            }

            // Use SqlBulkCopy - v7.0 automatically handles hidden columns
            using var bulkCopy = new SqlBulkCopy(connection);
            bulkCopy.DestinationTableName = "dbo.TemporalExample";
            bulkCopy.EnableStreaming = true;

            // Create sample data
            var dataTable = new DataTable();
            dataTable.Columns.Add("Id", typeof(int));
            dataTable.Columns.Add("Name", typeof(string));
            dataTable.Rows.Add(1, "Sample Data");

            bulkCopy.WriteToServer(dataTable);

            Console.WriteLine("SqlBulkCopy successfully handled temporal table with hidden columns (v7.0 feature)");

            // Cleanup
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "DROP TABLE dbo.TemporalExample;";
                command.ExecuteNonQuery();
            }
        }
    }
}
