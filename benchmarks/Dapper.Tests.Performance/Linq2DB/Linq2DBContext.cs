using LinqToDB;
using LinqToDB.Configuration;
using LinqToDB.Data;

namespace Dapper.Tests.Performance.Linq2Db
{
    public class Linq2DBContext : DataConnection
    {
        public Linq2DBContext(DataOptions options) : base(options)
        {
        }

        public ITable<Post> Posts => this.GetTable<Post>();
    }
}
