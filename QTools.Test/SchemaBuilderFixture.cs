using NUnit.Framework;
using QTools.Schema;
using System;

namespace QTools.Test
{
    [TestFixture]
    public class SchemaBuilderFixture
    {
        #region Test POCO

        public class QTestEntity
        {
            public bool BoolProp { get; set; }
            public bool? NullBoolProp { get; set; }
            public char CharProp { get; set; }
            public char? NullCharProp { get; set; }
            public byte ByteProp { get; set; }
            public byte? NullByteProp { get; set; }
            public short ShortProp { get; set; }
            public short? NullShortProp { get; set; }
            public int IntProp { get; set; }
            public int? NullIntProp { get; set; }
            public long LongProp { get; set; }
            public long? NullLongProp { get; set; }
            public float RealProp { get; set; }
            public float? NullRealProp { get; set; }
            public double DoubleProp { get; set; }
            public double? NullDoubleProp { get; set; }
            public decimal DecimalProp { get; set; }
            public decimal? NullDecimalProp { get; set; }
            public DateTime DateTimeProp { get; set; }
            public DateTime? NullDateTimeProp { get; set; }
            public TimeSpan TimeSpanProp { get; set; }
            public TimeSpan? NullTimeSpanProp { get; set; }
            [Date] // Adding the attribute forces a q-date instead of a q-datetime.
            public DateTime DateProp { get; set; }
            [Time] // Adding the attribute forces a q-time instead of a q-timespan.
            public TimeSpan TimeProp { get; set; }
            [Time]  // Adding the attribute forces a q-time
            public DateTime DateTimeAsTimeProp { get; set; }
            public string SymbolProp { get; set; }
            [String] // Adding the attribute forces a q-string instead of a q-symbol.
            public string StringProp { get; set; }
        }

        #endregion
    
        const string QTestEntityResult =
            ".qtools.test.qTestEntity:(" +
            "[]" +
            "boolProp:`boolean$()" +
            ";nullBoolProp:`boolean$()" +
            ";charProp:`char$()" +
            ";nullCharProp:`char$()" +
            ";byteProp:`byte$()" +
            ";nullByteProp:`byte$()" +
            ";shortProp:`short$()" +
            ";nullShortProp:`short$()" +
            ";intProp:`int$()" +
            ";nullIntProp:`int$()" +
            ";longProp:`long$()" +
            ";nullLongProp:`long$()" +
            ";realProp:`real$()" +
            ";nullRealProp:`real$()" +
            ";doubleProp:`float$()" +
            ";nullDoubleProp:`float$()" +
            ";decimalProp:`float$()" +
            ";nullDecimalProp:`float$()" +
            ";dateTimeProp:`datetime$()" +
            ";nullDateTimeProp:`datetime$()" +
            ";timeSpanProp:`timespan$()" +
            ";nullTimeSpanProp:`timespan$()" +
            ";dateProp:`date$()" +
            ";timeProp:`time$()" +
            ";dateTimeAsTimeProp:`time()" +
            ";symbolProp:`symbol$()" +
            ";stringProp:string()" +
            ")";

        [Test]
        public void TestSchemaBuilder()
        {
            var declaration = SchemaBuilder.DeclareEmptyTable(typeof(QTestEntity));
            Assert.That(declaration, Is.EqualTo(QTestEntityResult));
        }

        #region Keyed and Attributed POCO

        public class QAttributeTestEntity
        {
            [Key]
            public int Key1 { get; set; }
            [Key]
            public string Key2 { get; set; }
            [Sorted] // Can only be applied to atomic types
            public string ValueA { get; set; }
            [Parted] // Can only be applied to atomic types
            public string ValueB { get; set; }
            [Grouped] // Can only be applied to atomic types
            public string ValueC { get; set; }
        }

        #endregion

        const string QAttributeTestResult = 
            ".qtools.test.qAttributeTestEntity:(" +
            "[key1:`int$();key2:`symbol$()]" +
            "`s#valueA:`symbol$()" +
            ";`p#valueB:`symbol$()" +
            ";`g#valueC:`symbol$()" +
            ")";

        [Test]
        public void TestKeysAndAttribute()
        {
            var declaration = SchemaBuilder.DeclareEmptyTable(typeof(QAttributeTestEntity));
            Assert.That(declaration, Is.EqualTo(QAttributeTestResult));
        }
    }
}
