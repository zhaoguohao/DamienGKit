// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2007/11/24/calculating-elf-32-in-c-and-net

using System;
using System.Security.Cryptography;

namespace DamienG.Security.Cryptography
{
    /// <summary>
    /// Implements a 32-bit Alder hash algorithm compatible with zlib streams.
    /// </summary>
    public sealed class Adler32 : HashAlgorithm
    {
        private const UInt16 ModAdler = 65521;
        private const UInt16 DefaultFirst = 1;
        private const UInt16 DefaultSecond = 0;

        private UInt16 _first = DefaultFirst;
        private UInt16 _second = DefaultSecond;

        /// <summary>Create a new <see cref="Adler32"/> hasher.</summary>
        public Adler32()
        {
            if (!BitConverter.IsLittleEndian)
                throw new PlatformNotSupportedException("Not supported on Big Endian processors");
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            _first = DefaultFirst;
            _second = DefaultSecond;
        }

        /// <inheritdoc/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            (_first, _second) = CalculateHash(_first, _second, array, ibStart, cbSize);
        }

        /// <inheritdoc/>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt32ToBigEndianBytes(CalculateFinal(_first, _second));
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <inheritdoc/>
        public override int HashSize => 32;

        /// <summary>Calculate the Adler32 hash for <paramref name="buffer"/>.</summary>
        public static UInt32 Compute(byte[] buffer)
        {
            (UInt16 first, UInt16 second) = CalculateHash(DefaultFirst, DefaultSecond, buffer, 0, buffer.Length);
            return CalculateFinal(first, second);
        }
        
        private static UInt32 CalculateFinal(UInt16 first, UInt16 second)
            => (UInt32) (second << 16) | first;
        
        private static (UInt16, UInt16) CalculateHash(UInt16 first, UInt16 second, byte[] buffer, int start, int size)
        {
            for (var i = start; i < start + size; i++)
            {
                first = (UInt16) ((first + buffer[i]) % ModAdler);
                second = (UInt16) ((first + second) % ModAdler);
            }
            
            return (first, second);
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
