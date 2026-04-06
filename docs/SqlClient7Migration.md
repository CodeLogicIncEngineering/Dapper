# Microsoft.Data.SqlClient 7.0 Migration Guide

This guide covers the upgrade to Microsoft.Data.SqlClient 7.0 and how to leverage new features.

## Breaking Changes

### 1. Azure Dependencies Removed from Core Package

**Impact:** If you're using Entra ID (Azure Active Directory) authentication, you need to install an additional package.

**Action Required:**
- Install `Microsoft.Data.SqlClient.Extensions.Azure` package if using:
  - `ActiveDirectoryInteractive`
  - `ActiveDirectoryServicePrincipal`
  - `ActiveDirectoryManagedIdentity`
  - `ActiveDirectoryDefault`
  - `ActiveDirectoryPassword` (deprecated, see below)

**Not Applicable to Dapper:** This codebase does not use Entra ID authentication.

### 2. Deprecated ActiveDirectoryPassword

**Impact:** `SqlAuthenticationMethod.ActiveDirectoryPassword` is now marked `[Obsolete]`.

**Recommended Alternatives:**
- `ActiveDirectoryInteractive`
- `ActiveDirectoryServicePrincipal`
- `ActiveDirectoryManagedIdentity`
- `ActiveDirectoryDefault`

**Not Applicable to Dapper:** This authentication method is not used.

## New Features in v7.0

### 1. Enhanced Connection Resiliency

v7.0 includes improved retry logic and better handling of transient failures.

**Usage Example:**
```csharp
using Dapper;

var connectionString = SqlClientConfiguration.BuildOptimizedConnectionString(
    "Data Source=.;Initial Catalog=tempdb;Integrated Security=True",
    new SqlClientConfigurationOptions
    {
        EnableConnectionResiliency = true,
        ConnectRetryCount = 3,
        ConnectRetryInterval = 10
    });

using var connection = new SqlConnection(connectionString);
```

**Configurable Retry Logic:**
```csharp
var retryOption = SqlClientConfiguration.CreateRetryLogicOption(
    maxRetries: 3,
    deltaTime: TimeSpan.FromSeconds(1),
    maxTimeInterval: TimeSpan.FromSeconds(20));

var retryLogicProvider = SqlConfigurableRetryLogicManager.CreateExponentialRetryProvider(retryOption);

using var connection = new SqlConnection(connectionString);
connection.RetryLogicProvider = retryLogicProvider;
```

### 2. New App Context Switches

v7.0 introduces several app context switches for global behavior control:

#### Enable Multi-Subnet Failover by Default
```csharp
// Set in app startup or configuration
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault", true);
```

**When to use:** Always On Availability Groups or Azure SQL Database with geo-replication.

#### Ignore Server-Provided Failover Partner
```csharp
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.IgnoreServerProvidedFailoverPartner", true);
```

**When to use:** Basic Availability Groups where you want client-side control of failover.

#### Enable User Agent Feature
```csharp
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableUserAgent", true);
```

**When to use:** For better telemetry and diagnostics (opt-in for privacy).

### 3. Enhanced Routing for Read Replicas

v7.0 adds support for enhanced routing, enabling better load balancing across read replicas in Azure SQL Hyperscale.

**Configuration:**
```csharp
var builder = new SqlConnectionStringBuilder
{
    DataSource = "your-server.database.windows.net",
    InitialCatalog = "your-database",
    ApplicationIntent = ApplicationIntent.ReadOnly, // Route to read replica
    // v7.0 will automatically use enhanced routing if server supports it
};
```

### 4. Improved Diagnostic Events

v7.0 brings diagnostic events to .NET Framework (previously .NET Core only).

**Available on all platforms:**
- `SqlClientCommandBefore` / `SqlClientCommandAfter`
- `SqlClientConnectionOpenBefore` / `SqlClientConnectionOpenAfter`
- `SqlClientTransactionCommitBefore` / `SqlClientTransactionCommitAfter`
- And 9 more diagnostic event types

**Implementation Example:**
```csharp
using System.Diagnostics;
using Microsoft.Data.SqlClient.Diagnostics;

public class SqlClientDiagnosticsListener : IObserver<DiagnosticListener>
{
    public void OnNext(DiagnosticListener listener)
    {
        if (listener.Name == "SqlClientDiagnosticListener")
        {
            listener.Subscribe(new SqlEventObserver());
        }
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}

public class SqlEventObserver : IObserver<KeyValuePair<string, object>>
{
    public void OnNext(KeyValuePair<string, object> value)
    {
        if (value.Value is SqlClientCommandBefore commandBefore)
        {
            // Log command execution start
            Console.WriteLine($"Executing: {commandBefore.Command.CommandText}");
        }
        else if (value.Value is SqlClientCommandAfter commandAfter)
        {
            // Log command execution completion
            Console.WriteLine($"Completed in {commandAfter.Duration.TotalMilliseconds}ms");
        }
    }

    public void OnError(Exception error) { }
    public void OnCompleted() { }
}
```

### 5. SqlBulkCopy Hidden Column Support

v7.0 allows SqlBulkCopy to operate on hidden columns (e.g., temporal tables).

**No code changes required** - this is automatic behavior.

### 6. Exposed Baseline Transient Errors

v7.0 exposes the default list of transient error codes:

```csharp
using Dapper;

var transientErrors = SqlClientConfiguration.GetBaselineTransientErrors();
foreach (var errorCode in transientErrors)
{
    Console.WriteLine($"Transient error code: {errorCode}");
}
```

## Performance Improvements

v7.0 includes numerous performance improvements:

1. **Faster connection opening** - Optimized connection pool management
2. **Improved SqlStatistics timing** - More accurate performance metrics
3. **Always Encrypted optimizations** - Better performance for encrypted columns
4. **Reduced allocations** - Less GC pressure in hot paths

**No code changes required** - these are automatic improvements.

## Dependency Updates

### System.ValueTuple

Microsoft.Data.SqlClient 7.0 requires `System.ValueTuple >= 4.6.2` (updated from 4.6.1).

This has been updated in `Directory.Packages.props`.

## Testing Recommendations

1. **Connection Pool Behavior**: Test connection pooling under load
2. **Retry Logic**: Verify retry behavior with transient failures
3. **Performance**: Benchmark critical queries to validate improvements
4. **Diagnostics**: Enable diagnostic listeners in test environments
5. **Failover**: Test multi-subnet failover if using Always On AG

## Additional Resources

- [Official Release Notes](https://github.com/dotnet/SqlClient/blob/main/release-notes/7.0/7.0.0.md)
- [Microsoft.Data.SqlClient Documentation](https://docs.microsoft.com/sql/connect/ado-net/microsoft-ado-net-sql-server)
- [Configurable Retry Logic](https://docs.microsoft.com/sql/connect/ado-net/configurable-retry-logic)
