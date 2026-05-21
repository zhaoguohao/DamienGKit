using System.Collections.Generic;
using System.IO;
using System.Text;
using DamienG.IO;
using Xunit;

namespace DamienG.Tests.IO
{
    public class MulticastTextWriterTests
    {
        [Fact]
        public void EncodingIsDefault()
        {
            Assert.Equal(Encoding.Default, new MulticastTextWriter().Encoding);
        }

        [Fact]
        public void WriteCharArrayFansOutToAllWriters()
        {
            var a = new StringWriter();
            var b = new StringWriter();
            var multicast = new MulticastTextWriter(new List<TextWriter> { a, b });

            multicast.Write("hello world".ToCharArray(), 0, 11);

            Assert.Equal("hello world", a.ToString());
            Assert.Equal("hello world", b.ToString());
        }

        [Fact]
        public void WriteCharArrayHonoursIndexAndCount()
        {
            var a = new StringWriter();
            var multicast = new MulticastTextWriter(new List<TextWriter> { a });

            multicast.Write("hello world".ToCharArray(), 6, 5);

            Assert.Equal("world", a.ToString());
        }

        [Fact]
        public void AddAddsWriterToMulticast()
        {
            var a = new StringWriter();
            var b = new StringWriter();
            var multicast = new MulticastTextWriter(new List<TextWriter> { a });

            multicast.Add(b);
            multicast.Write("hi".ToCharArray(), 0, 2);

            Assert.Equal("hi", a.ToString());
            Assert.Equal("hi", b.ToString());
        }

        [Fact]
        public void RemoveRemovesWriterFromMulticast()
        {
            var a = new StringWriter();
            var b = new StringWriter();
            var multicast = new MulticastTextWriter(new List<TextWriter> { a, b });

            Assert.True(multicast.Remove(b));
            multicast.Write("hi".ToCharArray(), 0, 2);

            Assert.Equal("hi", a.ToString());
            Assert.Equal(string.Empty, b.ToString());
        }

        [Fact]
        public void RemoveReturnsFalseForUnknownWriter()
        {
            var multicast = new MulticastTextWriter();
            Assert.False(multicast.Remove(new StringWriter()));
        }

        [Fact]
        public void ConstructorCopiesProvidedWriters()
        {
            var source = new List<TextWriter> { new StringWriter() };
            var multicast = new MulticastTextWriter(source);

            source.Add(new StringWriter());

            multicast.Write("x".ToCharArray(), 0, 1);
            Assert.Equal("x", source[0].ToString());
            Assert.Equal(string.Empty, source[1].ToString());
        }
    }
}
