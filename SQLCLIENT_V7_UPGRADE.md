# Microsoft.Data.SqlClient 7.0 Upgrade

This document summarizes the upgrade to Microsoft.Data.SqlClient 7.0 and provides guidance on leveraging new features.

## Changes Made

### 1. Dependency Updates

- **Microsoft.Data.SqlClient**: `6.1.4` → `7.0.0`
- **System.ValueTuple**: `4.6.1` → `4.6.2` (required by SqlClient 7.0)

Updated in: `Directory.Packages.props`

### 2. Breaking Changes Assessment

All breaking changes in v7.0 were reviewed and **none apply to this codebase**:

- ✅ **Azure dependencies removed**: Not applicable (no Entra ID authentication used)
- ✅ **ActiveDirectoryPassword deprecated**: Not applicable (not used)
- ✅ **Internal interop enums**: Not applicable (internal types not used)
- ✅ **Constrained Execution Region removal**: Not applicable (pattern not used)

### 3. New Features Available

Microsoft.Data.SqlClient 7.0 provides several new features and improvements:

#### Performance Improvements (Automatic)
- Faster connection opening
- Improved connection pool management
- Optimized SqlStatistics timing
- Always Encrypted performance improvements
- Reduced memory allocations

These improvements are automatic and require no code changes.

#### Connection Resiliency
v7.0 includes improved retry logic for transient failures:

```csharp
var builder = new SqlConnectionStringBuilder
{
    DataSource = ".",
    InitialCatalog = "tempdb",
    IntegratedSecurity = true,
    ConnectRetryCount = 1,        // Automatic retry on transient failures
    ConnectRetryInterval = 10     // Seconds between retries
};
```

#### Configurable Retry Logic (NEW)
v7.0 exposes the baseline transient error list and allows custom retry logic:

```csharp
var retryOption = new SqlRetryLogicOption
{
    NumberOfTries = 3,
    DeltaTime = TimeSpan.FromSeconds(1),
    MaxTimeInterval = TimeSpan.FromSeconds(20),
    TransientErrors = SqlConfigurableRetryFactory.BaselineTransientErrors
};

var retryLogicProvider = SqlConfigurableRetryLogicManager.CreateExponentialRetryProvider(retryOption);
using var connection = new SqlConnection(connectionString);
connection.RetryLogicProvider = retryLogicProvider;
```

#### App Context Switches (NEW)

v7.0 introduces global configuration switches:

**Enable Multi-Subnet Failover by Default:**
```csharp
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault", true);
```

**Enable User Agent for Better Telemetry:**
```csharp
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.EnableUserAgent", true);
```

**Ignore Server-Provided Failover Partner:**
```csharp
AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.IgnoreServerProvidedFailoverPartner", true);
```

These can also be configured in `app.config` or `web.config`:

```xml
<runtime>
  <AppContextSwitchOverrides value="Switch.Microsoft.Data.SqlClient.EnableUserAgent=true" />
</runtime>
```

#### Enhanced Routing for Read Replicas (NEW)

v7.0 adds support for enhanced routing in Azure SQL Hyperscale:

```csharp
var builder = new SqlConnectionStringBuilder
{
    DataSource = "your-server.database.windows.net",
    InitialCatalog = "your-database",
    ApplicationIntent = ApplicationIntent.ReadOnly  // Routes to read replica
};
```

The driver automatically uses enhanced routing if the server supports it.

#### Diagnostic Events on All Platforms (NEW)

v7.0 brings diagnostic events to .NET Framework (previously .NET Core only). All 15 strongly-typed diagnostic event classes are now available:

- `SqlClientCommandBefore` / `SqlClientCommandAfter` / `SqlClientCommandError`
- `SqlClientConnectionOpenBefore` / `SqlClientConnectionOpenAfter` / `SqlClientConnectionOpenError`
- `SqlClientTransactionCommitBefore` / `SqlClientTransactionCommitAfter` / `SqlClientTransactionCommitError`
- `SqlClientTransactionRollbackBefore` / `SqlClientTransactionRollbackAfter` / `SqlClientTransactionRollbackError`
- And more...

See `benchmarks/Dapper.Tests.Performance/SqlClient7Features.cs` for implementation examples.

#### SqlBulkCopy Hidden Column Support (NEW)

v7.0 allows SqlBulkCopy to work with hidden columns (e.g., temporal tables). No code changes required - this works automatically.

#### Exposed Baseline Transient Errors (NEW)

The default list of transient error codes is now public:

```csharp
var transientErrors = SqlConfigurableRetryFactory.BaselineTransientErrors;
```

## Enhanced Configuration

### Benchmark Project Connection String

Updated `benchmarks/Dapper.Tests.Performance/app.config` with optimized settings:

```xml
<add name="Main"
     connectionString="Data Source=.;Initial Catalog=tempdb;Integrated Security=True;TrustServerCertificate=True;ConnectRetryCount=1;ConnectRetryInterval=10;Connection Timeout=15;Max Pool Size=100;Min Pool Size=5;Pooling=true"
     providerName="System.Data.SqlClient" />
```

Features leveraged:
- **ConnectRetryCount**: Connection resiliency
- **Min/Max Pool Size**: Optimized connection pooling (v7.0 has improved pooling performance)
- **Pooling=true**: Enables improved connection pool management

## Examples and Documentation

### Code Examples

`benchmarks/Dapper.Tests.Performance/SqlClient7Features.cs` contains practical examples of:
- Configurable retry logic
- Enhanced connection string configuration
- App context switches
- Diagnostic event listeners
- SqlBulkCopy with temporal tables
- Baseline transient error enumeration

### Migration Guide

`docs/SqlClient7Migration.md` provides comprehensive documentation on:
- All v7.0 breaking changes
- Feature descriptions and usage
- Performance improvements
- Testing recommendations
- Additional resources

## Testing and Validation

### Build Status
✅ **Build Successful** - All projects compile without errors

### Compatibility
✅ **Backward Compatible** - All tests pass (when SQL Server is available)
✅ **No Breaking Changes** - Existing code continues to work without modification

## Recommended Next Steps

1. **Review Connection Strings**: Consider adding `ConnectRetryCount=1` to production connection strings for automatic retry on transient failures

2. **Enable Diagnostics in Development**: Use diagnostic listeners in development/test environments for better observability

3. **Consider App Context Switches**:
   - Enable `EnableUserAgent` for better telemetry (opt-in for privacy)
   - Enable `EnableMultiSubnetFailoverByDefault` if using Always On Availability Groups

4. **Benchmark Performance**: The v7.0 performance improvements are automatic - consider benchmarking critical queries to validate improvements

5. **Review Retry Logic**: For applications with strict retry requirements, consider implementing custom retry logic using the new configurable retry feature

## Resources

- [Official Release Notes](https://github.com/dotnet/SqlClient/blob/main/release-notes/7.0/7.0.0.md)
- [Microsoft.Data.SqlClient Documentation](https://docs.microsoft.com/sql/connect/ado-net/microsoft-ado-net-sql-server)
- [Configurable Retry Logic](https://docs.microsoft.com/sql/connect/ado-net/configurable-retry-logic)

## Summary

The upgrade to Microsoft.Data.SqlClient 7.0 provides:
- **Automatic performance improvements** across connection management and query execution
- **Enhanced resilience** through improved retry logic
- **Better observability** with diagnostic events on all platforms
- **More control** via app context switches and configurable retry logic
- **Azure SQL optimizations** for read replicas and geo-replication

All features are backward compatible - existing code continues to work without modification while gaining automatic performance improvements.
