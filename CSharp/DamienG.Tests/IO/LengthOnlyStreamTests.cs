using System;
using System.IO;
using DamienG.IO;
using Xunit;

namespace DamienG.Tests.IO
{
    public class LengthOnlyStreamTests
    {
        [Fact]
        public void CapabilitiesReflectWriteOnlyStream()
        {
            var stream = new LengthOnlyStream();
            Assert.False(stream.CanRead);
            Assert.False(stream.CanSeek);
            Assert.True(stream.CanWrite);
        }

        [Fact]
        public void NewStreamHasZeroLength()
        {
            Assert.Equal(0, new LengthOnlyStream().Length);
        }

        [Fact]
        public void WriteAddsCountToLength()
        {
            var stream = new LengthOnlyStream();
            stream.Write(new byte[100], 0, 100);
            Assert.Equal(100, stream.Length);
        }

        [Fact]
        public void WriteAddsCountIgnoringOffset()
        {
            var stream = new LengthOnlyStream();
            stream.Write(new byte[100], 25, 50);
            Assert.Equal(50, stream.Length);
        }

        [Fact]
        public void MultipleWritesAccumulate()
        {
            var stream = new LengthOnlyStream();
            stream.Write(new byte[10], 0, 10);
            stream.Write(new byte[20], 0, 20);
            stream.Write(new byte[5], 0, 5);
            Assert.Equal(35, stream.Length);
        }

        [Fact]
        public void ResetReturnsLengthToZero()
        {
            var stream = new LengthOnlyStream();
            stream.Write(new byte[42], 0, 42);
            stream.Reset();
            Assert.Equal(0, stream.Length);
        }

        [Fact]
        public void SetLengthOverridesLength()
        {
            var stream = new LengthOnlyStream();
            stream.SetLength(1234);
            Assert.Equal(1234, stream.Length);
        }

        [Fact]
        public void ReadThrowsNotImplementedException()
        {
            Assert.Throws<NotImplementedException>(() => new LengthOnlyStream().Read(new byte[10], 0, 10));
        }

        [Fact]
        public void SeekReturnsZero()
        {
            Assert.Equal(0, new LengthOnlyStream().Seek(99, SeekOrigin.Begin));
        }

        [Fact]
        public void FlushDoesNotThrow()
        {
            new LengthOnlyStream().Flush();
        }

        [Fact]
        public void CountsLargeWrites()
        {
            var stream = new LengthOnlyStream();
            for (var i = 0; i < 1000; i++)
                stream.Write(new byte[1024], 0, 1024);
            Assert.Equal(1024L * 1000, stream.Length);
        }
    }
}
