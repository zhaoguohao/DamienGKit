// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

namespace DamienG.System.Binary
{
    /// <summary>
    /// Base class for encodings that convert between binary byte arrays and a text representation.
    /// </summary>
    public abstract class BinaryTextEncoding
    {
        /// <summary>
        /// Encode the supplied bytes into a text representation.
        /// </summary>
        /// <param name="bytes">The bytes to encode.</param>
        /// <returns>A string containing the encoded representation of <paramref name="bytes"/>.</returns>
        public abstract string Encode(byte[] bytes);

        /// <summary>
        /// Decode a text representation back into the original bytes.
        /// </summary>
        /// <param name="input">The encoded string to decode.</param>
        /// <returns>The decoded bytes.</returns>
        public abstract byte[] Decode(string input);
    }
}
