// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2006/08/08/calculating_crc32_in_c_and_net

using System;
using System.Security.Cryptography;

namespace DamienG.Security.Cryptography
{
    /// <summary>
    /// Implements a 32-bit CRC hash algorithm compatible with Zip etc.
    /// This version uses the Slice-by-8 technique developed by Intel at
    /// https://sourceforge.net/projects/slicing-by-8/ and while C#/.NET doesn't
    /// inline all of the performance benefits it does seem to perform slighly better.
    /// It processes 8 bytes at a time using 8 lookup tables so the table size is increased
    /// from 1KB to 8KB.  Data is read using two UInt32 conversions.  UInt64 did not seem to
    /// improve the speed.
    /// </summary>
    /// <remarks>
    /// Crc32 should only be used for backward compatibility with older file formats
    /// and algorithms. It is not secure enough for new applications.
    /// If you need to call multiple times for the same data either use the HashAlgorithm
    /// interface or remember that the result of one Compute call needs to be ~ (XOR) before
    /// being passed in as the seed for the next Compute call.
    /// </remarks>
    public sealed class Crc32Slice8 : HashAlgorithm
    {
        /// <summary>The default CRC32 polynomial (reversed representation of <c>0x04C11DB7</c>), compatible with Zip and similar formats.</summary>
        public const UInt32 DefaultPolynomial = 0xedb88320u;

        /// <summary>The default initial seed value (all bits set).</summary>
        public const UInt32 DefaultSeed = 0xffffffffu;

        private static UInt32[,] _defaultTable;

        private readonly UInt32 _seed;
        private readonly UInt32[,] _table;
        private UInt32 _hash;

        /// <summary>Create a new <see cref="Crc32Slice8"/> using <see cref="DefaultPolynomial"/> and <see cref="DefaultSeed"/>.</summary>
        public Crc32Slice8()
            : this(DefaultPolynomial, DefaultSeed)
        {
        }

        /// <summary>Create a new <see cref="Crc32Slice8"/> using a supplied polynomial and seed.</summary>
        /// <param name="polynomial">The polynomial to use in calculating.</param>
        /// <param name="seed">The initial seed to start from.</param>
        public Crc32Slice8(UInt32 polynomial, UInt32 seed)
        {
            if (!BitConverter.IsLittleEndian)
                throw new PlatformNotSupportedException("Not supported on Big Endian processors");

            _table = InitializeTable(polynomial);
            _seed = _hash = seed;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            _hash = _seed;
        }

        /// <inheritdoc/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hash = CalculateHash(_table, _hash, array, ibStart, cbSize);
        }

        /// <inheritdoc/>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(~_hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <inheritdoc/>
        public override int HashSize => 32;

        /// <summary>Calculate the CRC32 for <paramref name="buffer"/> using <see cref="DefaultPolynomial"/> and <see cref="DefaultSeed"/>.</summary>
        public static UInt32 Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        /// <summary>Calculate the CRC32 for <paramref name="buffer"/> using <see cref="DefaultPolynomial"/> and the supplied seed.</summary>
        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return Compute(DefaultPolynomial, seed, buffer);
        }

        /// <summary>Calculate the CRC32 for <paramref name="buffer"/> using the supplied polynomial and seed.</summary>
        public static UInt32 Compute(UInt32 polynomial, UInt32 seed, byte[] buffer)
        {
            return ~CalculateHash(InitializeTable(polynomial), seed, buffer, 0, buffer.Length);
        }

        private static UInt32[,] InitializeTable(UInt32 polynomial)
        {
            if (polynomial == DefaultPolynomial && _defaultTable != null)
                return _defaultTable;

            var table = new UInt32[8, 256];

            // Compute normal CRC table
            for (var i = 0; i < 256; i++)
            {
                var entry = (UInt32)i;
                for (var j = 0; j < 8; j++)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry >>= 1;
                table[0, i] = entry;
            }

            // Slicing by 8 table
            for (var i = 0; i < 256; i++)
            {
                table[1, i] = (table[0, i] >> 8) ^ table[0, table[0, i] & 0xFF];
                table[2, i] = (table[1, i] >> 8) ^ table[0, table[1, i] & 0xFF];
                table[3, i] = (table[2, i] >> 8) ^ table[0, table[2, i] & 0xFF];
                table[4, i] = (table[3, i] >> 8) ^ table[0, table[3, i] & 0xFF];
                table[5, i] = (table[4, i] >> 8) ^ table[0, table[4, i] & 0xFF];
                table[6, i] = (table[5, i] >> 8) ^ table[0, table[5, i] & 0xFF];
                table[7, i] = (table[6, i] >> 8) ^ table[0, table[6, i] & 0xFF];
            }

            if (polynomial == DefaultPolynomial)
                _defaultTable = table;

            return table;
        }

        private static UInt32 CalculateHash(UInt32[,] table, UInt32 seed, byte[] buffer, int start, int size)
        {
            var hash = seed;

            var length = size - start;
            var i = start;

            unchecked
            {
                while (length >= 8)
                {
                    var one = BitConverter.ToInt32(buffer, i) ^ hash;
                    var two = BitConverter.ToInt32(buffer, i + 4);
                    hash =
                        table[0, (two >> 24) & 0xFF] ^
                        table[1, (two >> 16) & 0xFF] ^
                        table[2, (two >> 8) & 0xFF] ^
                        table[3, two & 0xFF] ^
                        table[4, (one >> 24) & 0xFF] ^
                        table[5, (one >> 16) & 0xFF] ^
                        table[6, (one >> 8) & 0xFF] ^
                        table[7, one & 0xFF];
                    length -= 8;
                    i += 8;
                }

                while (length-- != 0)
                    hash = (hash >> 8) ^ table[0, buffer[i++] ^ hash & 0xff];
            }

            return hash;
        }

        private static byte[] UInt32ToBigEndianBytes(UInt32 uint32)
        {
            var result = BitConverter.GetBytes(uint32);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }
    }
}
