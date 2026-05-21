using System.Text;
using DamienG.IO;
using Xunit;

namespace DamienG.Tests.IO
{
    public class DebugTextWriterTests
    {
        [Fact]
        public void EncodingIsDefault()
        {
            Assert.Equal(Encoding.Default, new DebugTextWriter().Encoding);
        }

        [Fact]
        public void WriteStringDoesNotThrow()
        {
            new DebugTextWriter().Write("hello");
        }

        [Fact]
        public void WriteNullStringDoesNotThrow()
        {
            new DebugTextWriter().Write((string)null);
        }

        [Fact]
        public void WriteCharArrayDoesNotThrow()
        {
            new DebugTextWriter().Write("hello world".ToCharArray(), 6, 5);
        }

        [Fact]
        public void WriteLineDoesNotThrow()
        {
            new DebugTextWriter().WriteLine("hello");
        }
    }
}
