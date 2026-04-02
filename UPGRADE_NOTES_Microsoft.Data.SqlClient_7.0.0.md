# Microsoft.Data.SqlClient 7.0.0 Upgrade Notes

## Overview

This document details the upgrade of Microsoft.Data.SqlClient from version 6.1.4 to 7.0.0 in the Dapper project.

**Upgrade Date:** April 2, 2026
**Previous Version:** 6.1.4
**New Version:** 7.0.0

---

## Changes Made

### Package Version Updates

#### Directory.Packages.props

1. **Microsoft.Data.SqlClient**: 6.1.4 → 7.0.0
2. **System.ValueTuple**: 4.6.1 → 4.6.2 (required dependency)

---

## Breaking Changes in Microsoft.Data.SqlClient 7.0.0

### 1. Azure Dependencies Removed from Core Package

**Change:** Entra ID authentication (`ActiveDirectoryAuthenticationProvider` and related types) has been extracted into a new `Microsoft.Data.SqlClient.Extensions.Azure` package.

**Impact on this project:** ✅ **None**
- The Dapper project does not use ActiveDirectory authentication methods
- No code changes required
- No additional package dependencies needed

**For projects using Entra ID authentication:** Applications using Entra ID authentication must install `Microsoft.Data.SqlClient.Extensions.Azure` separately.

### 2. Deprecated `SqlAuthenticationMethod.ActiveDirectoryPassword`

**Change:** The `ActiveDirectoryPassword` (ROPC flow) method is now marked `[Obsolete]` and will generate compiler warnings.

**Impact on this project:** ✅ **None**
- Not used in the Dapper codebase

**Migration path for affected projects:**
- Migrate to `ActiveDirectoryInteractive`
- Migrate to `ActiveDirectoryServicePrincipal`
- Migrate to `ActiveDirectoryManagedIdentity`
- Migrate to `ActiveDirectoryDefault`

### 3. Internal Interop Enums Visibility Reverted

**Change:** Internal interop enums (`IoControlCodeAccess` and `IoControlTransferType`) that were accidentally made public have been reverted to internal visibility.

**Impact on this project:** ✅ **None**
- Not used in the Dapper codebase

### 4. Removed Constrained Execution Region (CER) Error Handling

**Change:** Removed `Constrained Execution Region` error handling blocks and associated `SqlConnection` cleanup.

**Impact on this project:** ✅ **None**
- Internal implementation detail, no API changes

---

## Dependency Resolution

### System.ValueTuple Version Conflict

**Issue:** Microsoft.Data.SqlClient 7.0.0 requires System.ValueTuple >= 4.6.2, but the project was pinned to 4.6.1.

**Error Message:**
```
error NU1605: Detected package downgrade: System.ValueTuple from 4.6.2 to 4.6.1
```

**Resolution:** Updated System.ValueTuple from 4.6.1 to 4.6.2 in Directory.Packages.props

---

## New Features in 7.0.0

The following features are now available (but not required for this upgrade):

1. **Custom SSPI Authentication Support**
   - `SspiContextProvider` abstract class
   - `SqlConnection.SspiContextProvider` property
   - Enables cross-domain Kerberos negotiation and NTLM username/password authentication

2. **Enhanced Routing Support**
   - TDS feature for server-side connection redirection
   - Azure SQL Hyperscale read replica load balancing

3. **New App Context Switches**
   - `Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault`
   - `Switch.Microsoft.Data.SqlClient.IgnoreServerProvidedFailoverPartner`
   - `Switch.Microsoft.Data.SqlClient.EnableUserAgent`

4. **Improved Diagnostics**
   - `SqlClientDiagnosticListener` enabled for `SqlCommand` on .NET Framework
   - 15 strongly-typed diagnostic event classes now available on .NET Framework
   - Enhanced tracing capabilities

5. **Performance Improvements**
   - SqlStatistics timing improvements
   - Always Encrypted scenario optimizations
   - Connection opening performance enhancements

6. **SqlBulkCopy Enhancement**
   - Now supports operations on hidden columns

---

## Build Verification

### Build Status: ✅ **Success**

All projects in the Dapper solution built successfully with the updated dependencies:

- Dapper (net461, netstandard2.0, net8.0, net10.0)
- Dapper.StrongName (net461, netstandard2.0, net8.0)
- Dapper.Tests (net481, net8.0, net9.0, net10.0)
- Dapper.Tests.Performance (net462, net8.0, net10.0)
- Dapper.SqlBuilder
- Dapper.Rainbow
- Dapper.EntityFramework
- Dapper.EntityFramework.StrongName
- Dapper.ProviderTools
- docs

### Target Frameworks Tested

- .NET Framework 4.61, 4.62, 4.81
- .NET Standard 2.0
- .NET 5.0
- .NET 8.0
- .NET 9.0
- .NET 10.0

---

## Migration Checklist

- [x] Updated Microsoft.Data.SqlClient to 7.0.0
- [x] Updated System.ValueTuple to 4.6.2
- [x] Verified no usage of deprecated ActiveDirectory authentication methods
- [x] Verified no usage of accidentally-public internal interop enums
- [x] Confirmed build succeeds on all target frameworks
- [x] Verified no additional package dependencies required (no Azure authentication used)
- [x] Committed changes to version control

---

## Compatibility Notes

### No Code Changes Required

The upgrade from 6.1.4 to 7.0.0 required **zero code changes** in the Dapper project because:

1. **No Azure Authentication:** The project does not use Entra ID/ActiveDirectory authentication
2. **No Internal APIs:** The project does not use accidentally-exposed internal APIs
3. **Standard SQL Authentication:** Only standard SQL Server authentication and connection features are used
4. **Clean Dependency Tree:** Only the System.ValueTuple version needed updating

### Projects That May Require Code Changes

Projects using the following features will need additional work:

1. **Entra ID Authentication**
   - Add `Microsoft.Data.SqlClient.Extensions.Azure` package reference
   - Update using statements if needed

2. **ActiveDirectoryPassword Authentication**
   - Migrate to alternative authentication methods
   - Address `[Obsolete]` compiler warnings

3. **Internal Interop Enums**
   - Remove any references to `IoControlCodeAccess` or `IoControlTransferType`

---

## Related Packages Released

Microsoft released the following companion packages alongside 7.0.0:

- **Microsoft.Data.SqlClient.Extensions.Abstractions 1.0.0** - Shared types between core driver and extensions
- **Microsoft.Data.SqlClient.Extensions.Azure 1.0.0** - Entra ID authentication support
- **Microsoft.Data.SqlClient.Internal.Logging 1.0.0** - Shared ETW tracing infrastructure
- **Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider 7.0.0** - Always Encrypted with Azure Key Vault

---

## References

- **Release Notes:** [Microsoft.Data.SqlClient 7.0.0 Release Notes](https://github.com/dotnet/sqlclient/blob/main/CHANGELOG.md#Stable-Release-700---2026-03-17)
- **Project Site:** [https://aka.ms/sqlclientproject](https://aka.ms/sqlclientproject)
- **GitHub Repository:** [https://github.com/dotnet/sqlclient](https://github.com/dotnet/sqlclient)

---

## Commit Information

**Commit Hash:** 4292cc225f027cbaefe4e1c81808586b1d52db2f
**Author:** CodeLogicAI <codelogic-ai@codelogic.com>
**Date:** Thu Apr 2 20:02:03 2026 +0000

**Commit Message:**
```
Update System.ValueTuple to 4.6.2 for Microsoft.Data.SqlClient 7.0.0 compatibility

Microsoft.Data.SqlClient 7.0.0 requires System.ValueTuple >= 4.6.2, but the
project was previously pinned to 4.6.1, causing a package downgrade error.

This commit updates the System.ValueTuple version to 4.6.2 in Directory.Packages.props
to resolve the dependency conflict and ensure compatibility with the upgraded
Microsoft.Data.SqlClient library.
```

---

## Support and Questions

For questions or issues related to this upgrade:

1. **Dapper Issues:** [https://github.com/DapperLib/Dapper/issues](https://github.com/DapperLib/Dapper/issues)
2. **Microsoft.Data.SqlClient Issues:** [https://github.com/dotnet/SqlClient/issues](https://github.com/dotnet/SqlClient/issues)

---

**Document Version:** 1.0
**Last Updated:** April 2, 2026
