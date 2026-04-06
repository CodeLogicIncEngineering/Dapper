using System;

namespace Dapper.Tests.Performance
{
    [ServiceStack.DataAnnotations.Alias("Posts")]
    [LinqToDB.Mapping.Table(Name = "Posts")]
    public class Post
    {
        [LinqToDB.Mapping.PrimaryKey, LinqToDB.Mapping.Identity]
        public int Id { get; set; }

        // In v6.x, nullability is inferred from the type, so Nullable/NotNull attributes are optional
        [LinqToDB.Mapping.Column]
        public string Text { get; set; }

        [LinqToDB.Mapping.Column]
        public DateTime CreationDate { get; set; }

        [LinqToDB.Mapping.Column]
        public DateTime LastChangeDate { get; set; }

        // Nullable value types automatically infer nullability in v6.x
        [LinqToDB.Mapping.Column]
        public int? Counter1 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter2 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter3 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter4 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter5 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter6 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter7 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter8 { get; set; }

        [LinqToDB.Mapping.Column]
        public int? Counter9 { get; set; }
    }
}
