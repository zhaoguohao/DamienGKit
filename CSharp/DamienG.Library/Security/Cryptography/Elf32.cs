// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2007/11/24/calculating-elf-32-in-c-and-net

using System;
using System.Security.Cryptography;

namespace DamienG.Security.Cryptography
{
    /// <summary>
    /// Implements a 32-bit ELF hash algorithm compatible with ELF binary format.
    /// </summary>
    public sealed class Elf32 : HashAlgorithm
    {
        private UInt32 _hash;

        /// <summary>Create a new <see cref="Elf32"/> hasher.</summary>
        public Elf32()
        {
            if (!BitConverter.IsLittleEndian)
                throw new PlatformNotSupportedException();

            _hash = 0;
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            _hash = 0;
        }

        /// <inheritdoc/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            _hash = CalculateHash(_hash, array, ibStart, cbSize);
        }

        /// <inheritdoc/>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(_hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <inheritdoc/>
        public override int HashSize => 32;

        /// <summary>Calculate the ELF32 hash for <paramref name="buffer"/> starting with a zero seed.</summary>
        public static UInt32 Compute(byte[] buffer)
        {
            return CalculateHash(0, buffer, 0, buffer.Length);
        }

        /// <summary>Calculate the ELF32 hash for <paramref name="buffer"/> starting with the supplied <paramref name="seed"/>.</summary>
        public static UInt32 Compute(UInt32 seed, byte[] buffer)
        {
            return CalculateHash(seed, buffer, 0, buffer.Length);
        }

        private static UInt32 CalculateHash(UInt32 seed, byte[] buffer, int start, int size)
        {
            var hash = seed;
            for (var i = start; i < start + size; i++)
            {
                hash = (hash << 4) + buffer[i];
                var work = hash & 0xf0000000u;
                hash ^= work >> 24;
                hash &= ~work;
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
