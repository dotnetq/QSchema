using NUnit.Framework;
using System;

namespace QTools.Test
{
    [TestFixture]
    public class SchemaFixture
    {
        [Test]
        public void TestQBoolean()
        {
            var s = new QBooleanSchema();
            Assert.That(s.QNotation((bool?)null), Is.EqualTo("0b"));
            Assert.That(s.QNotation(true), Is.EqualTo("1b"));
            Assert.That(s.QNotation(false), Is.EqualTo("0b"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`boolean$()"));
        }
        [Test]
        public void TestQByte()
        {
            var s = new QByteSchema();
            Assert.That(s.QNotation((byte?)null), Is.EqualTo("0x00"));
            Assert.That(s.QNotation((byte)1), Is.EqualTo("0x01"));
            Assert.That(s.QNotation((byte)0), Is.EqualTo("0x00"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`byte$()"));
        }
        [Test]
        public void TestQShort()
        {
            var s = new QShortSchema();
            Assert.That(s.QNotation((short?)null), Is.EqualTo("0Nh"));
            Assert.That(s.QNotation((short)1), Is.EqualTo("1h"));
            Assert.That(s.QNotation((short)0), Is.EqualTo("0h"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`short$()"));
        }
        [Test]
        public void TestQInt()
        {
            var s = new QIntSchema();
            Assert.That(s.QNotation((int?)null), Is.EqualTo("0Ni"));
            Assert.That(s.QNotation(1), Is.EqualTo("1i"));
            Assert.That(s.QNotation(0), Is.EqualTo("0i"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`int$()"));
        }
        [Test]
        public void TestQLong()
        {
            var s = new QLongSchema();
            Assert.That(s.QNotation((long?)null), Is.EqualTo("0Nj"));
            Assert.That(s.QNotation((long)1), Is.EqualTo("1j"));
            Assert.That(s.QNotation((long)0), Is.EqualTo("0j"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`long$()"));
        }
        [Test]
        public void TestQReal()
        {
            var s = new QRealSchema();
            Assert.That(s.QNotation((float?)null), Is.EqualTo("0Ne"));
            Assert.That(s.QNotation((float)1), Is.EqualTo("1e"));
            Assert.That(s.QNotation((float)0), Is.EqualTo("0e"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`real$()"));
        }
        [Test]
        public void TestQFloat()
        {
            var s = new QFloatSchema();
            Assert.That(s.QNotation((double?)null), Is.EqualTo("0n"));
            Assert.That(s.QNotation((double)1), Is.EqualTo("1"));
            Assert.That(s.QNotation((double)0), Is.EqualTo("0"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`float$()"));
        }
        [Test]
        public void TestQDecimal()
        {
            var s = new QDecimalSchema();
            Assert.That(s.QNotation((decimal?)null), Is.EqualTo("0n"));
            Assert.That(s.QNotation((decimal)1), Is.EqualTo("1"));
            Assert.That(s.QNotation((decimal)0), Is.EqualTo("0"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`float$()"));
        }
        [Test]
        public void TestQString()
        {
            var s = new QStringSchema();
            Assert.That(s.QNotation((string)null), Is.EqualTo("\"\""));
            Assert.That(s.QNotation("1"), Is.EqualTo("\"1\""));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:string()"));
        }
        [Test]
        public void TestQDate()
        {
            var s = new QDateSchema();
            Assert.That(s.QNotation(null), Is.EqualTo("0Nd"));
            Assert.That(s.QNotation(new DateTime(2018,03,01)), Is.EqualTo("2018.03.01"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`date$()"));
        }
        [Test]
        public void TestQTime()
        {
            var s = new QTimeSchema();
            Assert.That(s.QNotation(null), Is.EqualTo("0Nt"));
            Assert.That(s.QNotation(new TimeSpan(0,13,14,15,16)), Is.EqualTo("13:14:15.016"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`time$()"));
        }
        [Test]
        public void TestQDateTime()
        {
            var s = new QDateTimeSchema();
            Assert.That(s.QNotation(null), Is.EqualTo("0Nz"));
            Assert.That(s.QNotation(new DateTime(2018, 03, 01, 13, 14, 15,16)), Is.EqualTo("2018.03.01T13:14:15.016"));
            Assert.That(s.AsTableColumn("col"), Is.EqualTo("col:`datetime$()"));
        }
    }
}
