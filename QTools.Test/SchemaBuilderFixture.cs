using NUnit.Framework;
using QTools.Schema;

namespace QTools.Test
{
    [TestFixture]
    public class SchemaBuilderFixture
    {
        public class QTestEntity
        {
            [Key]
            public string Id { get; set; }
            [Unique]
            public bool BoolProperty { get; set; }
            [Sorted]
            public bool? NullBoolProperty { get; set; }
            [String]
            public string Memo { get; set; }
        }

        [Test]
        public void TestSchemaBuilder()
        {
            var declaration = SchemaBuilder.DeclareEmptyTable(typeof(QTestEntity));

        }
    }
}
