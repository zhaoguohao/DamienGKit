// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2007/11/19/calculating-crc-64-in-c-and-net

using System;
using System.Security.Cryptography;

namespace DamienG.Security.Cryptography
{
    /// <summary>
    /// Implements a 64-bit CRC hash algorithm for a given polynomial.
    /// </summary>
    /// <remarks>
    /// For ISO 3309 compliant 64-bit CRC's use Crc64Iso.
    /// </remarks>
    public class Crc64 : HashAlgorithm
    {
        /// <summary>The default initial seed value (zero).</summary>
        public const UInt64 DefaultSeed = 0x0;

        private readonly UInt64[] _table;

        private readonly UInt64 _seed;
        private UInt64 _hash;

        /// <summary>Create a new <see cref="Crc64"/> using the supplied polynomial and <see cref="DefaultSeed"/>.</summary>
        /// <param name="polynomial">The polynomial to use in calculating.</param>
        public Crc64(UInt64 polynomial)
            :  this(polynomial, DefaultSeed)
        {
        }

        /// <summary>Create a new <see cref="Crc64"/> using the supplied polynomial and seed.</summary>
        /// <param name="polynomial">The polynomial to use in calculating.</param>
        /// <param name="seed">The initial seed to start from.</param>
        public Crc64(UInt64 polynomial, UInt64 seed)
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
            _hash = CalculateHash(_hash, _table, array, ibStart, cbSize);
        }

        /// <inheritdoc/>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt64ToBigEndianBytes(_hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <inheritdoc/>
        public override int HashSize => 64;

        /// <summary>Core CRC64 calculation over a slice of <paramref name="buffer"/>.</summary>
        /// <param name="seed">The initial hash value to start from.</param>
        /// <param name="table">The polynomial-derived lookup table.</param>
        /// <param name="buffer">The byte array to read from.</param>
        /// <param name="start">The starting offset into <paramref name="buffer"/>.</param>
        /// <param name="size">The number of bytes to read.</param>
        /// <returns>The accumulated CRC64.</returns>
        protected static UInt64 CalculateHash(UInt64 seed, UInt64[] table, byte[] buffer, int start, int size)
        {
            var hash = seed;
            for (var i = start; i < start + size; i++)
            {
                hash = (hash >> 8) ^ table[(buffer[i] ^ hash) & 0xff];
            }

            return hash;
        }

        private static byte[] UInt64ToBigEndianBytes(UInt64 value)
        {
            var result = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        private static UInt64[] InitializeTable(UInt64 polynomial)
        {
            if (polynomial == Crc64Iso.Iso3309Polynomial && Crc64Iso.Table != null)
                return Crc64Iso.Table;

            var createTable = CreateTable(polynomial);

            if (polynomial == Crc64Iso.Iso3309Polynomial)
                Crc64Iso.Table = createTable;

            return createTable;
        }

        /// <summary>Build a 256-entry CRC64 lookup table for the supplied <paramref name="polynomial"/>.</summary>
        protected static ulong[] CreateTable(ulong polynomial)
        {
            var createTable = new UInt64[256];
            for (var i = 0; i < 256; ++i)
            {
                var entry = (UInt64) i;
                for (var j = 0; j < 8; ++j)
                    if ((entry & 1) == 1)
                        entry = (entry >> 1) ^ polynomial;
                    else
                        entry >>= 1;
                createTable[i] = entry;
            }
            return createTable;
        }
    }

    /// <summary>ISO 3309 compliant 64-bit CRC, a common pre-defined <see cref="Crc64"/> variant.</summary>
    public class Crc64Iso : Crc64
    {
        internal static UInt64[] Table;

        /// <summary>The ISO 3309 standard CRC64 polynomial.</summary>
        public const UInt64 Iso3309Polynomial = 0xD800000000000000;

        /// <summary>Create a new <see cref="Crc64Iso"/> using <see cref="DefaultSeed"/>.</summary>
        public Crc64Iso()
            : base(Iso3309Polynomial)
        {
        }

        /// <summary>Create a new <see cref="Crc64Iso"/> using the supplied seed.</summary>
        /// <param name="seed">The initial seed to start from.</param>
        public Crc64Iso(UInt64 seed)
            : base(Iso3309Polynomial, seed)
        {
        }

        /// <summary>Calculate the ISO 3309 CRC64 for <paramref name="buffer"/> using <see cref="DefaultSeed"/>.</summary>
        public static UInt64 Compute(byte[] buffer)
        {
            return Compute(DefaultSeed, buffer);
        }

        /// <summary>Calculate the ISO 3309 CRC64 for <paramref name="buffer"/> using the supplied seed.</summary>
        public static UInt64 Compute(UInt64 seed, byte[] buffer)
        {
            if (Table == null)
                Table = CreateTable(Iso3309Polynomial);

            return CalculateHash(seed, Table, buffer, 0, buffer.Length);
        }
    }
}
