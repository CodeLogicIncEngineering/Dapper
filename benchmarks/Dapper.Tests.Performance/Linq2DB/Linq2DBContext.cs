using LinqToDB;
using LinqToDB.Configuration;

namespace Dapper.Tests.Performance.Linq2Db
{
    public class Linq2DBContext : LinqToDB.Data.DataConnection
    {
        public Linq2DBContext(DataOptions options) : base(options)
        {
        }

        public ITable<Post> Posts => this.GetTable<Post>();
    }
}
