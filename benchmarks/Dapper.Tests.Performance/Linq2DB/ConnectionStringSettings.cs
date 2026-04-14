using System;
using LinqToDB.Configuration;

namespace Dapper.Tests.Performance.Linq2Db
{
    /// <summary>
    /// Legacy configuration class for linq2db. Obsolete as of linq2db v6.
    /// Use DataOptions builder pattern instead. See Benchmarks.Linq2DB.cs for the modern approach.
    /// </summary>
    [Obsolete("Use DataOptions builder pattern instead. This class is retained for reference only.")]
    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }
}