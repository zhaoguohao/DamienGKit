using System;
using DamienG.Security.Cryptography;
using Xunit;

namespace DamienG.Tests.Security.Cryptography
{
    public class Crc32Tests : BaseHashAlgorithmTests
    {
        [Fact]
        public void StaticDefaultSeedAndPolynomialWithShortAsciiString()
        {
            var actual = Crc32.Compute(SimpleBytesAscii);

            Assert.Equal(0x519025e9U, actual);
        }

        [Fact]
        public void StaticDefaultSeedAndPolynomialWithShortAsciiString2()
        {
            var actual = Crc32.Compute(SimpleBytes2Ascii);

            Assert.Equal(0x6ee3ad88U, actual);
        }

        [Fact]
        public void InstanceDefaultSeedAndPolynomialWith12KBinaryFile()
        {
            var hash = GetTestFileHash(Binary12KFileName, new Crc32());

            Assert.Equal(0x9865b070U, GetBigEndianUInt32(hash));
        }

        [Fact]
        public void InstanceDefaultSeedAndPolynomialWith1MBinaryFile()
        {
            var hash = GetTestFileHash(Binary1MFileName, new Crc32());

            Assert.Equal(0xaffeed9fUL, GetBigEndianUInt32(hash));
        }

        [Fact]
        public void KnownPolynomialAndSeed()
        {
            // CRC32 with DefaultPolynomial (0xEDB88320) and DefaultSeed (0xFFFFFFFF)
            // Input "123456789" is a standard check value
            var bytes = "123456789"u8.ToArray();
            var actual = Crc32.Compute(bytes);
            
            Assert.Equal(0xCBF43926U, actual);
        }
    }
}
