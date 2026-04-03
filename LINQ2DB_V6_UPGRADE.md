# linq2db.SqlServer v6 Upgrade Summary

## Overview

This document summarizes the changes required to upgrade from `linq2db.SqlServer` version 5.4.1.9 to version 6.2.1.

## Breaking Changes

### linq2db.SqlServer Package Structure Change

In **linq2db v6**, the `linq2db.SqlServer` package underwent a significant architectural change:

- **v5.x and earlier**: The `linq2db.SqlServer` package included both T4 scaffolding templates AND the runtime library (`linq2db`) as a transitive dependency.
- **v6.x**: The `linq2db.SqlServer` package is now a **template-only package** containing only T4 scaffolding templates. It no longer includes the runtime library as a dependency.

This means that projects upgrading to v6 must explicitly reference the `linq2db` runtime package.

## Required Changes

### 1. Add linq2db Runtime Package

**File: `Directory.Packages.props`**

Added explicit reference to the `linq2db` runtime package:

```xml
<PackageVersion Include="linq2db" Version="6.2.1" />
<PackageVersion Include="linq2db.SqlServer" Version="6.2.1" />
```

### 2. Update Project References

**File: `benchmarks/Dapper.Tests.Performance/Dapper.Tests.Performance.csproj`**

Added explicit package reference to the runtime library:

```xml
<PackageReference Include="linq2db" />
<PackageReference Include="linq2db.SqlServer" />
```

## Why This Change Was Necessary

Without the explicit `linq2db` package reference, the project fails to build with errors such as:

```
error CS0246: The type or namespace name 'LinqToDB' could not be found
(are you missing a using directive or an assembly reference?)
```

This occurs because:
1. The code uses types from the `LinqToDB` namespace (e.g., `DataConnection`, `ILinqToDBSettings`, `ITable<T>`)
2. These types are provided by the `linq2db` runtime package
3. In v6, `linq2db.SqlServer` no longer brings in the runtime package automatically

## Affected Code

The following files use linq2db runtime types and require the `linq2db` package:

- `benchmarks/Dapper.Tests.Performance/Benchmarks.Linq2DB.cs`
- `benchmarks/Dapper.Tests.Performance/Linq2DB/Linq2DBContext.cs`
- `benchmarks/Dapper.Tests.Performance/Linq2DB/Linq2DbSettings.cs`
- `benchmarks/Dapper.Tests.Performance/Linq2DB/ConnectionStringSettings.cs`
- `benchmarks/Dapper.Tests.Performance/Post.cs`

## Target Frameworks

The changes support all project target frameworks:
- .NET Framework 4.6.2 (net462)
- .NET 8.0 (net8.0)
- .NET 10.0 (net10.0)

## New Features in v6

According to the release notes, linq2db v6 includes:

- .NET 10 support
- Support for new .NET 6-10 LINQ operators: `CountBy`, `Index`, `MaxBy`, `MinBy`, `ExceptBy`, `UnionBy`, `IntersectBy`
- Improved translation of aggregate and window functions
- `string.Join` translation support
- Various SQL generation improvements
- Enhanced support for SQL Server 2025 VECTOR types

## Verification

The upgrade has been verified by:
1. Successful restoration of all NuGet packages
2. Successful compilation for all target frameworks (net462, net8.0, net10.0)
3. Zero build warnings or errors

## Migration Guide for Other Projects

If you have other projects using `linq2db.SqlServer` and are upgrading to v6:

1. Add the `linq2db` package to your package dependencies with the same version as `linq2db.SqlServer`
2. Add a `PackageReference` to `linq2db` in your project file
3. Rebuild and verify compilation

## References

- [linq2db v6.0.0 Release Notes](https://github.com/linq2db/linq2db/releases/tag/v6.0.0)
- [linq2db v6.2.1 Release Notes](https://github.com/linq2db/linq2db/releases/tag/v6.2.1)
- [linq2db Documentation](https://linq2db.github.io/)
