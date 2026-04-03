# Changes for Microsoft.Data.SqlClient 7.0.0 Upgrade

**Date:** April 2, 2026
**Pull Request:** Update dependency Microsoft.Data.SqlClient to v7
**Branch:** renovate/microsoft.data.sqlclient-7.x

---

## Summary

This document summarizes the specific changes made to upgrade Microsoft.Data.SqlClient from version 6.1.4 to 7.0.0 in the Dapper project.

---

## Files Modified

### 1. Directory.Packages.props

**Location:** `/work/Directory.Packages.props`

#### Changes Made:

```diff
- <PackageVersion Include="Microsoft.Data.SqlClient" Version="6.1.4" />
+ <PackageVersion Include="Microsoft.Data.SqlClient" Version="7.0.0" />
```

```diff
- <PackageVersion Include="System.ValueTuple" Version="4.6.1" />
+ <PackageVersion Include="System.ValueTuple" Version="4.6.2" />
```

#### Reason:
- **Microsoft.Data.SqlClient**: Direct upgrade as specified in the Renovate PR
- **System.ValueTuple**: Required to resolve dependency conflict. Microsoft.Data.SqlClient 7.0.0 requires System.ValueTuple >= 4.6.2, which conflicted with the existing 4.6.1 version

---

## Code Changes

### No Code Modifications Required

**Finding:** After analyzing all files that reference Microsoft.Data.SqlClient, no code changes were necessary because:

1. **No Azure/Entra ID Authentication Used**
   - The codebase does not use `ActiveDirectoryAuthenticationProvider` or related ActiveDirectory authentication methods
   - Therefore, no need to add `Microsoft.Data.SqlClient.Extensions.Azure` package
   - Files checked:
     - `/work/tests/Dapper.Tests/TestBase.cs`
     - `/work/benchmarks/Dapper.Tests.Performance/Program.cs`
     - `/work/benchmarks/Dapper.Tests.Performance/Benchmarks.cs`
     - `/work/benchmarks/Dapper.Tests.Performance/Benchmarks.HandCoded.cs`
     - `/work/benchmarks/Dapper.Tests.Performance/LegacyTests.cs`
     - `/work/benchmarks/Dapper.Tests.Performance/SqlDataReaderHelper.cs`

2. **No Deprecated APIs Used**
   - No usage of `SqlAuthenticationMethod.ActiveDirectoryPassword`
   - No usage of accidentally-public internal interop enums (`IoControlCodeAccess`, `IoControlTransferType`)

3. **Standard SQL Server Connectivity Only**
   - All usage is through standard `SqlConnection`, `SqlCommand`, and factory patterns
   - No advanced authentication or internal APIs used

---

## Build Verification

### Commands Executed:

```bash
export PATH="$HOME/.local/dotnet:$PATH"
export DOTNET_ROOT=$HOME/.local/dotnet
dotnet restore Dapper.sln
dotnet build Dapper.sln --no-restore
```

### Results:

✅ **Build Status:** SUCCESS

**Output Summary:**
- All projects restored successfully
- All projects built successfully
- No errors encountered
- Only warnings: Known vulnerability in AWSSDK.Core 4.0.0.14 (unrelated to this upgrade)

**Projects Built:**
- Dapper (net461, netstandard2.0, net8.0, net10.0)
- Dapper.StrongName (net461, netstandard2.0, net8.0)
- Dapper.Tests (net481, net8.0, net9.0, net10.0)
- Dapper.Tests.Performance (net462, net8.0, net10.0)
- Dapper.SqlBuilder (net461, netstandard2.0)
- Dapper.Rainbow (net461, netstandard2.0, net5.0)
- Dapper.EntityFramework (net461)
- Dapper.EntityFramework.StrongName (net461)
- Dapper.ProviderTools (net461, netstandard2.0, net8.0)
- docs (net8.0)

---

## Error Resolution

### Issue Encountered:

During `dotnet restore`, the following error occurred:

```
error NU1605: Detected package downgrade: System.ValueTuple from 4.6.2 to 4.6.1
  Dapper.Tests -> Microsoft.Data.SqlClient 7.0.0 -> System.ValueTuple (>= 4.6.2)
  Dapper.Tests -> System.ValueTuple (>= 4.6.1)
```

### Root Cause:

Microsoft.Data.SqlClient 7.0.0 has a dependency on System.ValueTuple >= 4.6.2, but the project explicitly specified System.ValueTuple 4.6.1 in Directory.Packages.props. NuGet detected this as a package downgrade and failed the restore.

### Resolution:

Updated System.ValueTuple from 4.6.1 to 4.6.2 in Directory.Packages.props, which satisfied the dependency requirement and resolved the build error.

---

## Testing

### Test Execution:

A build verification was performed across all target frameworks. The build system confirmed:
- All projects compile successfully
- All multi-targeting configurations build correctly
- No runtime changes required

### Test Coverage:

The existing test suite in `/work/tests/Dapper.Tests/` already covers:
- Connection handling with both System.Data.SqlClient and Microsoft.Data.SqlClient
- Parameter handling
- Provider-specific tests
- Wrapped reader tests
- Single row tests

No test modifications were required because the upgrade did not introduce breaking API changes affecting the Dapper codebase.

---

## Commits

### Commit 1: Dependency Update
**Hash:** 4292cc225f027cbaefe4e1c81808586b1d52db2f
**Message:** Update System.ValueTuple to 4.6.2 for Microsoft.Data.SqlClient 7.0.0 compatibility
**Files Changed:** Directory.Packages.props (1 insertion, 1 deletion)

### Commit 2: Documentation
**Hash:** 8b5d5ef
**Message:** Add upgrade documentation for Microsoft.Data.SqlClient 7.0.0
**Files Changed:** UPGRADE_NOTES_Microsoft.Data.SqlClient_7.0.0.md (231 insertions)

---

## Breaking Changes Assessment

### Breaking Changes in 7.0.0:

1. ❌ **Azure Dependencies Removal** - NOT APPLICABLE
   - No Entra ID authentication in codebase

2. ❌ **ActiveDirectoryPassword Deprecation** - NOT APPLICABLE
   - Not used in codebase

3. ❌ **Internal Interop Enums** - NOT APPLICABLE
   - Not used in codebase

4. ❌ **Constrained Execution Region Removal** - NOT APPLICABLE
   - Internal implementation detail, no API impact

### Impact: ZERO BREAKING CHANGES

All breaking changes in Microsoft.Data.SqlClient 7.0.0 affect features not used by the Dapper project.

---

## Dependency Tree Changes

### Before:

```
Microsoft.Data.SqlClient 6.1.4
├── System.ValueTuple 4.6.1 (project override)
└── [Azure.Identity and transitive dependencies]
```

### After:

```
Microsoft.Data.SqlClient 7.0.0
├── System.ValueTuple 4.6.2 (required version)
└── [No Azure dependencies - removed in 7.0.0]
```

**Net Change:**
- Cleaner dependency tree (Azure dependencies removed from core package)
- One transitive dependency version bump (System.ValueTuple)
- No new dependencies added

---

## Configuration Changes

### No Configuration Required

The following app context switches are available in 7.0.0 but not needed for this upgrade:
- `Switch.Microsoft.Data.SqlClient.EnableMultiSubnetFailoverByDefault`
- `Switch.Microsoft.Data.SqlClient.IgnoreServerProvidedFailoverPartner`
- `Switch.Microsoft.Data.SqlClient.EnableUserAgent`

Default settings remain appropriate for the Dapper use case.

---

## Rollback Plan

If rollback is required:

```bash
# Revert the dependency changes
git revert 8b5d5ef  # Remove documentation
git revert 4292cc2  # Revert System.ValueTuple update
git revert b50edba  # Revert Microsoft.Data.SqlClient update
```

Or manually edit Directory.Packages.props:
```xml
<PackageVersion Include="Microsoft.Data.SqlClient" Version="6.1.4" />
<PackageVersion Include="System.ValueTuple" Version="4.6.1" />
```

---

## Verification Checklist

- [x] Package versions updated in Directory.Packages.props
- [x] Dependency conflicts resolved (System.ValueTuple)
- [x] Solution restores without errors
- [x] Solution builds without errors
- [x] All target frameworks compile successfully
- [x] No code changes required
- [x] No configuration changes required
- [x] No additional package references required
- [x] Build tools not committed to repository
- [x] Documentation created
- [x] Changes committed with descriptive messages

---

## Next Steps

1. **Review** - Have team review the changes
2. **Test** - Run full test suite in CI/CD pipeline
3. **Merge** - Merge the PR when approved
4. **Monitor** - Watch for any runtime issues post-deployment

---

## Additional Resources

- **Comprehensive Upgrade Guide:** `UPGRADE_NOTES_Microsoft.Data.SqlClient_7.0.0.md`
- **Official Release Notes:** https://github.com/dotnet/sqlclient/blob/main/CHANGELOG.md#Stable-Release-700---2026-03-17
- **Migration Guide:** https://aka.ms/sqlclientproject

---

**Prepared by:** CodeLogicAI
**Document Version:** 1.0
**Last Updated:** April 2, 2026
