using System;
using System.Collections.Generic;
using DamienG.System.Text;
using Xunit;

namespace DamienG.Tests.System.Text
{
    public class JsonEstimatorTests
    {
        [Fact]
        public void NullEstimatesAsFour()
        {
            Assert.Equal(4, JsonEstimator.Estimate(null, true));
        }

        [Fact]
        public void BoolTrueEstimatesAsFour()
        {
            Assert.Equal(4, JsonEstimator.Estimate(true, true));
        }

        [Fact]
        public void BoolFalseEstimatesAsFive()
        {
            Assert.Equal(5, JsonEstimator.Estimate(false, true));
        }

        [Theory]
        [InlineData((byte)0, 1)]
        [InlineData((byte)9, 1)]
        [InlineData((byte)10, 2)]
        [InlineData((byte)200, 3)]
        [InlineData((byte)255, 3)]
        public void ByteEstimateMatchesDigitCount(byte value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Theory]
        [InlineData((sbyte)0, 1)]
        [InlineData((sbyte)-1, 2)]
        [InlineData((sbyte)-10, 3)]
        [InlineData((sbyte)127, 3)]
        [InlineData((sbyte)-128, 4)]
        public void SByteEstimateMatchesDigitCount(sbyte value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Fact]
        public void CharAsciiEstimatesAsThree()
        {
            // "x" = 3
            Assert.Equal(3, JsonEstimator.Estimate('x', true));
        }

        [Fact]
        public void CharRequiringEscapeEstimatesAsFour()
        {
            // "\n" = 4, "\"" = 4, "\\" = 4
            Assert.Equal(4, JsonEstimator.Estimate('\n', true));
            Assert.Equal(4, JsonEstimator.Estimate('"', true));
            Assert.Equal(4, JsonEstimator.Estimate('\\', true));
        }

        [Fact]
        public void CharControlEstimatesAsEight()
        {
            // "" = 8 (two quotes + six-byte escape)
            Assert.Equal(8, JsonEstimator.Estimate('', true));
        }

        [Fact]
        public void CharNonAsciiCountsUtf8Bytes()
        {
            Assert.Equal(4, JsonEstimator.Estimate('é', true));   // 2-byte UTF-8 + quotes
            Assert.Equal(5, JsonEstimator.Estimate('漢', true));  // 3-byte UTF-8 + quotes
        }

        [Fact]
        public void GuidEstimatesAs38()
        {
            Assert.Equal(38, JsonEstimator.Estimate(Guid.NewGuid(), true));
        }

        [Fact]
        public void DateTimeEstimatesAs35()
        {
            Assert.Equal(35, JsonEstimator.Estimate(DateTime.UtcNow, true));
        }

        [Fact]
        public void DateTimeOffsetEstimatesAs35()
        {
            Assert.Equal(35, JsonEstimator.Estimate(DateTimeOffset.UtcNow, true));
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(7, 1)]
        [InlineData(99, 2)]
        [InlineData(-5, 2)]
        [InlineData(1000, 4)]
        [InlineData(-1000, 5)]
        public void Int32EstimateMatchesStringLength(int value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Fact]
        public void Int16EstimateMatchesStringLength()
        {
            Assert.Equal(6, JsonEstimator.Estimate((short)-12345, true));
        }

        [Fact]
        public void Int64EstimateMatchesStringLength()
        {
            Assert.Equal(19, JsonEstimator.Estimate(1234567890123456789L, true));
        }

        [Fact]
        public void UInt16Estimate()
        {
            Assert.Equal(5, JsonEstimator.Estimate((ushort)65535, true));
        }

        [Fact]
        public void UInt32Estimate()
        {
            Assert.Equal(10, JsonEstimator.Estimate(4000000000u, true));
        }

        [Fact]
        public void UInt64Estimate()
        {
            Assert.Equal(20, JsonEstimator.Estimate(ulong.MaxValue, true));
        }

        [Fact]
        public void StringEstimateIsLengthPlusQuotes()
        {
            Assert.Equal(7, JsonEstimator.Estimate("hello", true));
            Assert.Equal(2, JsonEstimator.Estimate("", true));
        }

        [Theory]
        [InlineData("a\"b", 6)]      // "a\"b"     = 6
        [InlineData("a\\b", 6)]      // "a\\b"     = 6
        [InlineData("a\nb", 6)]      // "a\nb"     = 6
        [InlineData("a\tb", 6)]      // "a\tb"     = 6
        [InlineData("ab", 10)] // "ab" = 10
        public void StringEstimateAccountsForEscapes(string value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Theory]
        [InlineData("é", 4)]      // "é"  → 1×2 UTF-8 + 2
        [InlineData("漢", 5)]     // "漢" → 1×3 UTF-8 + 2
        [InlineData("😀", 6)]     // "😀" → surrogate pair → 4 UTF-8 + 2
        [InlineData("aé漢", 8)]   // 1 + 2 + 3 + 2
        public void StringEstimateCountsUtf8Bytes(string value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Theory]
        [InlineData("1", 1)]
        [InlineData("1.0", 3)]
        [InlineData("123.45", 6)]
        [InlineData("-5", 2)]
        [InlineData("0.0001", 6)]
        public void DecimalEstimateMatchesStringLength(string literal, int expected)
        {
            var d = decimal.Parse(literal, global::System.Globalization.CultureInfo.InvariantCulture);
            Assert.Equal(expected, JsonEstimator.Estimate(d, true));
        }

        [Fact]
        public void DoubleEstimateMatchesStringLength()
        {
            Assert.Equal(4, JsonEstimator.Estimate(1.25, true));
        }

        [Fact]
        public void FloatEstimateMatchesStringLength()
        {
            Assert.Equal(4, JsonEstimator.Estimate(1.25f, true));
        }

        [Fact]
        public void EmptyEnumerableEstimatesAsTwo()
        {
            Assert.Equal(2, JsonEstimator.Estimate(new int[0], true));
        }

        [Fact]
        public void SingleItemArrayEstimatesAsThree()
        {
            // "[1]"
            Assert.Equal(3, JsonEstimator.Estimate(new[] { 1 }, true));
        }

        [Fact]
        public void MultiItemArrayMatchesSerializedLength()
        {
            // "[1,2,3]" = 7 chars
            Assert.Equal(7, JsonEstimator.Estimate(new[] { 1, 2, 3 }, true));
        }

        [Fact]
        public void EmptyDictionaryEstimatesAsTwo()
        {
            Assert.Equal(2, JsonEstimator.Estimate(new Dictionary<string, int>(), true));
        }

        [Fact]
        public void DictionaryEstimateMatchesSerializedLength()
        {
            // {"a":1,"b":2} = 13
            var dict = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            Assert.Equal(13, JsonEstimator.Estimate(dict, true));
        }

        [Fact]
        public void DictionarySkipsNullValuesWhenIncludeNullsFalse()
        {
            var dict = new Dictionary<string, string> { ["a"] = "x", ["b"] = null };
            // includeNulls = true:  {"a":"x","b":null} = 18
            // includeNulls = false: {"a":"x"}          = 9
            Assert.Equal(9, JsonEstimator.Estimate(dict, false));
        }

        [Fact]
        public void ObjectEstimateUsesProperties()
        {
            // {"Name":"Bob","Age":30} = 23
            var obj = new Person { Name = "Bob", Age = 30 };
            Assert.Equal(23, JsonEstimator.Estimate(obj, true));
        }

        [Fact]
        public void ObjectSkipsNullPropertiesWhenIncludeNullsFalse()
        {
            // includeNulls=false → {"Age":30} = 10
            var obj = new Person { Name = null, Age = 30 };
            Assert.Equal(10, JsonEstimator.Estimate(obj, false));
        }

        [Fact]
        public void ObjectIncludesNullsWhenRequested()
        {
            // {"Name":null,"Age":30} = 22
            var obj = new Person { Name = null, Age = 30 };
            Assert.Equal(22, JsonEstimator.Estimate(obj, true));
        }

        [Fact]
        public void TimeSpanZeroEstimatesAsTen()
        {
            // "00:00:00" = 10 chars including surrounding quotes.
            Assert.Equal(10, JsonEstimator.Estimate(TimeSpan.Zero, true));
        }

        [Fact]
        public void TimeSpanWithDaysAndFractionMatchesSerialized()
        {
            // "c" format: "1.02:03:04.0050000" = 18, plus 2 quotes = 20.
            var ts = new TimeSpan(1, 2, 3, 4, 5);
            Assert.Equal(20, JsonEstimator.Estimate(ts, true));
        }

        [Fact]
        public void UriEstimateMatchesStringForm()
        {
            // "https://example.com/" = 22.
            Assert.Equal(22, JsonEstimator.Estimate(new Uri("https://example.com/"), true));
        }

        [Fact]
        public void UriEscapesAreCounted()
        {
            // %20 inside an URI is already escaped; no JSON escapes required → length+2.
            var uri = new Uri("https://example.com/a%20b");
            Assert.Equal(uri.ToString().Length + 2, JsonEstimator.Estimate(uri, true));
        }

        [Theory]
        [InlineData(Color.Red, 1)]    // 0
        [InlineData(Color.Green, 1)]  // 1
        [InlineData(Color.Blue, 1)]   // 2
        [InlineData(Color.Big, 3)]    // 100
        public void EnumEstimateIsNumericDigitCount(Color value, int expected)
        {
            Assert.Equal(expected, JsonEstimator.Estimate(value, true));
        }

        [Fact]
        public void EmptyByteArrayEstimatesAsTwo()
        {
            Assert.Equal(2, JsonEstimator.Estimate(new byte[0], true));
        }

        [Theory]
        [InlineData(1, 6)]    // 1 byte → 4 base64 chars + 2 quotes
        [InlineData(2, 6)]    // 2 bytes → 4 base64 chars + 2 quotes
        [InlineData(3, 6)]    // 3 bytes → 4 base64 chars + 2 quotes
        [InlineData(4, 10)]   // 4 bytes → 8 base64 chars + 2 quotes
        [InlineData(100, 138)] // 100 bytes → 136 base64 chars + 2 quotes
        public void ByteArrayEstimateMatchesBase64Length(int byteCount, int expected)
        {
            var bytes = new byte[byteCount];
            Assert.Equal(expected, JsonEstimator.Estimate(bytes, true));
        }

        [Fact]
        public void SelfReferencingObjectDoesNotStackOverflow()
        {
            var node = new Node { Name = "root" };
            node.Self = node;
            // {"Name":"root","Self":{},"Other":null} = 38 (cyclic re-entry on Self collapses to {}).
            Assert.Equal(38, JsonEstimator.Estimate(node, true));
        }

        [Fact]
        public void SelfReferencingListDoesNotStackOverflow()
        {
            var list = new List<object> { 1 };
            list.Add(list);
            // [1,[]] = 6
            Assert.Equal(6, JsonEstimator.Estimate(list, true));
        }

        [Fact]
        public void SelfReferencingDictionaryDoesNotStackOverflow()
        {
            var dict = new Dictionary<string, object> { ["a"] = 1 };
            dict["self"] = dict;
            // {"a":1,"self":{}} = 17
            Assert.Equal(17, JsonEstimator.Estimate(dict, true));
        }

        [Fact]
        public void MutualCycleDoesNotStackOverflow()
        {
            var a = new Node { Name = "a" };
            var b = new Node { Name = "b" };
            a.Other = b;
            b.Other = a;
            // a → {"Name":"a","Other":{"Name":"b","Other":{}}} — just ensure no overflow.
            var size = JsonEstimator.Estimate(a, true);
            Assert.True(size > 0);
        }

        private class Node
        {
            public string Name { get; set; }
            public Node Self { get; set; }
            public Node Other { get; set; }
        }

        public enum Color
        {
            Red = 0,
            Green = 1,
            Blue = 2,
            Big = 100
        }

        private class Person
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }
    }
}
