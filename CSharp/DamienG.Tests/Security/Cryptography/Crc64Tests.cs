using System;
using DamienG.Security.Cryptography;
using Xunit;

namespace DamienG.Tests.Security.Cryptography
{
    public class Crc64Tests : BaseHashAlgorithmTests
    {
        [Fact]
        public void IsoStaticDefaultSeedAndPolynomialWithShortAsciiString()
        {
            var actual = Crc64Iso.Compute(SimpleBytesAscii);

            Assert.Equal(0x7e210eb1b03e5a1dUL, actual);
        }

        [Fact]
        public void IsoStaticDefaultSeedAndPolynomialWithShortAsciiString2()
        {
            var actual = Crc64Iso.Compute(SimpleBytes2Ascii);

            Assert.Equal(0x416b4150508661eeUL, actual);
        }

        [Fact]
        public void IsoInstanceDefaultSeedAndPolynomialWith12KBinaryFile()
        {
            var hash = GetTestFileHash(Binary12KFileName, new Crc64Iso());

            Assert.Equal(0x9614cf8ea0ef8E63UL, GetBigEndianUInt64(hash));
        }


        [Fact]
        public void IsoInstanceDefaultSeedAndPolynomialWith1MBinaryFile()
        {
            var hash = GetTestFileHash(Binary1MFileName, new Crc64Iso());

            Assert.Equal(0xcccd937b6f682d4fUL, GetBigEndianUInt64(hash));
        }

        [Fact]
        public void KnownPolynomialAndSeed()
        {
            // CRC64-ISO with DefaultPolynomial (0xD800000000000000) and DefaultSeed (0)
            var bytes = "123456789"u8.ToArray();
            var actual = Crc64Iso.Compute(bytes);
            
            Assert.Equal(5090661014116757502UL, actual);
        }
    }
}