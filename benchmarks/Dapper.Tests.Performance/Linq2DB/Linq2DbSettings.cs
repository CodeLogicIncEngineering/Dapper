using System;
using System.Collections.Generic;
using System.Linq;
using LinqToDB.Configuration;

namespace Dapper.Tests.Performance.Linq2Db
{
    /// <summary>
    /// Legacy configuration class for linq2db. Obsolete as of linq2db v6.
    /// Use DataOptions builder pattern instead. See Benchmarks.Linq2DB.cs for the modern approach.
    /// </summary>
    [Obsolete("Use DataOptions builder pattern instead. This class is retained for reference only.")]
    public class Linq2DBSettings : ILinqToDBSettings
    {
        private readonly string _connectionString;
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "SqlServer";
        public string DefaultDataProvider => "SqlServer";

        public Linq2DBSettings(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "SqlServer",
                        ProviderName = "SqlServer",
                        ConnectionString = _connectionString
                    };
            }
        }
    }
}
