// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Text;

namespace DamienG.System.Binary
{
    /// <summary>
    /// Flags controlling which whitespace characters should not be encoded by <see cref="QuotedPrintable"/>.
    /// </summary>
    [Flags]
    public enum DoNotEncode
    {
        /// <summary>Leave tab characters (<c>0x09</c>) as-is rather than encoding them.</summary>
        Tab = 0x01,
        /// <summary>Leave space characters (<c>0x20</c>) as-is rather than encoding them.</summary>
        Space = 0x02
    }

    /// <summary>
    /// A <see cref="BinaryTextEncoding"/> implementing the Quoted-Printable encoding used in MIME (RFC 2045).
    /// </summary>
    public class QuotedPrintable : BinaryTextEncoding
    {
        private readonly bool _encodeSpaces = true;
        private readonly bool _encodeTabs = true;
        private const int MaxLineLength = 76;

        /// <summary>
        /// Create a new <see cref="QuotedPrintable"/> encoder that encodes both spaces and tabs.
        /// </summary>
        public QuotedPrintable()
        {
        }

        /// <summary>
        /// Create a new <see cref="QuotedPrintable"/> encoder leaving the specified whitespace characters unencoded.
        /// </summary>
        /// <param name="doNotEncode">Whitespace characters to leave unencoded.</param>
        public QuotedPrintable(DoNotEncode doNotEncode)
        {
            _encodeTabs = !doNotEncode.HasFlag(DoNotEncode.Tab);
            _encodeSpaces = !doNotEncode.HasFlag(DoNotEncode.Space);
        }

        /// <inheritdoc/>
        public override string Encode(byte[] bytes)
        {
            var sb = new StringBuilder();

            var column = 0;
            string nextOutput = null;

            foreach (var b in bytes)
            {
                nextOutput = ShouldEncode(b) ? "=" + b.ToString("X2") : Char.ToString((char)b);

                CheckForEndOfLine();
                sb.Append(nextOutput);
                column += nextOutput.Length;
            }

            if (nextOutput != null && nextOutput.Length == 1 && nextOutput[0] <= 32)
                sb.Append("=");

            return sb.ToString();

            void CheckForEndOfLine()
            {
                if (column + nextOutput.Length >= MaxLineLength)
                {
                    sb.Append("=\n");
                    column = 0;
                }
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="input"/> ends with a truncated <c>=</c> escape or contains an invalid hex digit.</exception>
        public override byte[] Decode(string input)
        {
            var output = new List<byte>();
            for (var textIndex = 0; textIndex < input.Length; textIndex++)
            {
                var t = input[textIndex];
                if (t == '=')
                {
                    textIndex++;
                    switch (input.Length - textIndex)
                    {
                        case 1:
                            throw new ArgumentOutOfRangeException(nameof(input), "Only one character found after = sign - is data truncated?");

                        case 0:
                            break;

                        default:
                            output.Add((byte)((Hex(input[textIndex++]) << 4) + Hex(input[textIndex])));
                            break;
                    }
                }
                else
                    output.Add((byte)t);
            }

            return output.ToArray();
        }

        private bool ShouldEncode(byte b)
        {
            if (b == ' ')
                return _encodeSpaces;
            if (b == '\t')
                return _encodeTabs;

            return b < 33 || b > 126 || b == '=';
        }

        private static int Hex(char a)
        {
            if (a >= '0' && a <= '9')
                return a - '0';

            if (a >= 'a' && a <= 'f')
                return a - 'a' + 10;

            if (a >= 'A' && a <= 'F')
                return a - 'A' + 10;

            throw new ArgumentOutOfRangeException(nameof(a), string.Format("Character {0} is not hexadecimal", a));
        }
    }
}
