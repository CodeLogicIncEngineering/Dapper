using BenchmarkDotNet.Attributes;

using System;
using System.Linq;
using Dapper.Tests.Performance.Linq2Db;
using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;
using LinqToDB.DataProvider.SqlServer;
using System.ComponentModel;

namespace Dapper.Tests.Performance
{
    [Description("LINQ to DB")]
    public class LinqToDBBenchmarks : BenchmarkBase // note To not 2 because the "2" confuses BDN CLI
    {
        private Linq2DBContext _dbContext;

        private static readonly Func<Linq2DBContext, int, Post> compiledQuery = CompiledQuery.Compile((Linq2DBContext db, int id) => db.Posts.First(c => c.Id == id));

        [GlobalSetup]
        public void Setup()
        {
            BaseSetup();
            // Use modern DataOptions pattern for linq2db v6
            var options = new DataOptions()
                .UseSqlServer(_connection.ConnectionString);
            _dbContext = new Linq2DBContext(options);
        }

        [Benchmark(Description = "First")]
        public Post First()
        {
            Step();
            return _dbContext.Posts.First(p => p.Id == i);
        }

        [Benchmark(Description = "First (Compiled)")]
        public Post Compiled()
        {
            Step();
            return compiledQuery(_dbContext, i);
        }

        [Benchmark(Description = "Query<T>")]
        public Post Query()
        {
            Step();
            return _dbContext.Query<Post>("select * from Posts where Id = @id", new { id = i }).First();
        }
    }
}
