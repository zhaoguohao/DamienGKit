// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2010/10/17/enums-better-syntax-improved-performance-and-tryparse-in-net-3-5

using System;
using System.Collections.Generic;
using System.Linq;

namespace DamienG.System
{
    /// <summary>
    /// Strongly typed version of Enum with Parsing and performance improvements.
    /// </summary>
    /// <typeparam name="T">Type of Enum</typeparam>
    public static class Enum<T> where T : struct, IConvertible
    {
        private static readonly IEnumerable<T> all = Enum.GetValues(typeof (T)).Cast<T>();
        private static readonly Dictionary<string, T> insensitiveNames = all.ToDictionary(k => Enum.GetName(typeof (T), k)?.ToUpperInvariant());
        private static readonly Dictionary<string, T> sensitiveNames = all.ToDictionary(k => Enum.GetName(typeof (T), k));
        private static readonly Dictionary<int, T> values = all.ToDictionary(k => Convert.ToInt32(k));
        private static readonly Dictionary<T, string> names = all.ToDictionary(k => k, v => v.ToString());

        /// <summary>Returns true when <paramref name="value"/> is a defined value of the enum.</summary>
        public static bool IsDefined(T value) => names.Keys.Contains(value);

        /// <summary>Returns true when <paramref name="value"/> is the case-sensitive name of a defined enum value.</summary>
        public static bool IsDefined(string value) => sensitiveNames.Keys.Contains(value);

        /// <summary>Returns true when <paramref name="value"/> is the underlying integer of a defined enum value.</summary>
        public static bool IsDefined(int value) => values.Keys.Contains(value);

        /// <summary>Returns all defined values of the enum.</summary>
        public static IEnumerable<T> GetValues() => all;

        /// <summary>Returns the names of all defined values of the enum.</summary>
        public static string[] GetNames() => names.Values.ToArray();

        /// <summary>Returns the name of the supplied <paramref name="value"/>, or <c>null</c> if it is not a defined enum value.</summary>
        public static string GetName(T value)
        {
            names.TryGetValue(value, out var name);
            return name;
        }

        /// <summary>Parses <paramref name="value"/> as the case-sensitive name of an enum value.</summary>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is not a defined name.</exception>
        public static T Parse(string value)
        {
            if (!sensitiveNames.TryGetValue(value, out var parsed))
                throw new ArgumentException("Value is not one of the named constants defined for the enumeration", nameof(value));
            return parsed;
        }

        /// <summary>Parses <paramref name="value"/> as an enum name, optionally ignoring case.</summary>
        /// <param name="value">The name to parse.</param>
        /// <param name="ignoreCase">Whether to match names case-insensitively.</param>
        /// <exception cref="ArgumentException">If <paramref name="value"/> is not a defined name.</exception>
        public static T Parse(string value, bool ignoreCase)
        {
            if (!ignoreCase)
                return Parse(value);

            if (!insensitiveNames.TryGetValue(value.ToUpperInvariant(), out var parsed))
                throw new ArgumentException("Value is not one of the named constants defined for the enumeration", nameof(value));
            return parsed;
        }

        /// <summary>Attempts to parse <paramref name="value"/> as the case-sensitive name of an enum value.</summary>
        public static bool TryParse(string value, out T returnValue) => sensitiveNames.TryGetValue(value, out returnValue);

        /// <summary>Attempts to parse <paramref name="value"/> as an enum name, optionally ignoring case.</summary>
        public static bool TryParse(string value, bool ignoreCase, out T returnValue)
        {
            return ignoreCase
                       ? insensitiveNames.TryGetValue(value.ToUpperInvariant(), out returnValue)
                       : TryParse(value, out returnValue);
        }

        /// <summary>Parses <paramref name="value"/> as a case-sensitive enum name, returning <c>null</c> if empty or not found.</summary>
        public static T? ParseOrNull(string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            if (sensitiveNames.TryGetValue(value, out var foundValue))
                return foundValue;

            return null;
        }

        /// <summary>Parses <paramref name="value"/> as an enum name (optionally ignoring case), returning <c>null</c> if empty or not found.</summary>
        public static T? ParseOrNull(string value, bool ignoreCase)
        {
            if (!ignoreCase)
                return ParseOrNull(value);

            if (string.IsNullOrEmpty(value))
                return null;

            if (insensitiveNames.TryGetValue(value.ToUpperInvariant(), out var foundValue))
                return foundValue;

            return null;
        }

        /// <summary>Returns the enum value with the supplied underlying integer, or <c>null</c> if no such value is defined.</summary>
        public static T? CastOrNull(int value)
        {
            if (values.TryGetValue(value, out var foundValue))
                return foundValue;

            return null;
        }

        /// <summary>Returns the individual flag values present in the supplied <paramref name="flagEnum"/>.</summary>
        public static IEnumerable<T> GetFlags(T flagEnum)
        {
            var flagInt = Convert.ToInt32(flagEnum);
            return all.Where(e => (Convert.ToInt32(e) & flagInt) != 0);
        }

        /// <summary>Combines the supplied <paramref name="flags"/> into a single enum value via bitwise OR.</summary>
        public static T SetFlags(IEnumerable<T> flags)
        {
            var combined = flags.Aggregate(0, (current, flag) => current | Convert.ToInt32(flag));
            return values.TryGetValue(combined, out var result) ? result : default;
        }
    }
}
