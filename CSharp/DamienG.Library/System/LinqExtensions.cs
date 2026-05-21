// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections.Generic;
using System.Linq;

namespace DamienG.System
{
    /// <summary>
    /// Various LINQ extensions for <see cref="IEnumerable{T}"/> sequences.
    /// </summary>
    public static class LinqExtensions
    {
        private static readonly Random random = new Random();

        /// <summary>
        /// Order <paramref name="source"/> so that items appear in the same order that their
        /// <paramref name="property"/> values appear in <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="T">Element type of the source sequence.</typeparam>
        /// <typeparam name="TProperty">Type of the property used to look up ordering positions.</typeparam>
        /// <param name="source">The sequence to order.</param>
        /// <param name="property">Selector that maps each element to a key present in <paramref name="sequence"/>.</param>
        /// <param name="sequence">The ordered list of keys defining the desired output order.</param>
        /// <returns><paramref name="source"/> reordered to match the order of <paramref name="sequence"/>.</returns>
        public static IEnumerable<T> OrderBySequence<T, TProperty>(this IEnumerable<T> source, Func<T, TProperty> property, IEnumerable<TProperty> sequence)
        {
            var sequenceList = sequence.ToList();
            var sequenceDictionary = sequenceList.ToDictionary(s => s, sequenceList.IndexOf);
            return source.OrderBy(s => sequenceDictionary[property(s)]);
        }

        /// <summary>
        /// Returns the requested page of items from <paramref name="source"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">The sequence to page through.</param>
        /// <param name="page">The one-based page number to return.</param>
        /// <param name="pageSize">The maximum number of items per page.</param>
        /// <returns>The items belonging to the requested page.</returns>
        /// <exception cref="ArgumentException">If <paramref name="page"/> or <paramref name="pageSize"/> is less than 1.</exception>
        public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            if (page < 1 || pageSize < 1)
                throw new ArgumentException("Must be 1 or greater", page < 1 ? "page" : "pageSize");

            return source.Skip(--page * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Returns <paramref name="value"/> if it is found in <paramref name="source"/>; otherwise the default value for <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="value">The value to look for.</param>
        /// <returns><paramref name="value"/> if present, otherwise <c>default(T)</c>.</returns>
        public static T ContainsOrDefault<T>(this IEnumerable<T> source, T value)
        {
            return source.Contains(value) ? value : default;
        }

        /// <summary>
        /// Return a random element from the supplied <paramref name="collection"/>.
        /// </summary>
        /// <typeparam name="T">Element type.</typeparam>
        /// <param name="collection">Collection to pick from.</param>
        /// <returns>A uniformly random element of <paramref name="collection"/>.</returns>
        public static T RandomElement<T>(this ICollection<T> collection)
        {
            return collection.ElementAt(random.Next(collection.Count));
        }

        /// <summary>
        /// Return the result of <paramref name="selector"/> applied to the first element that matches
        /// <paramref name="predicate"/>, or <c>default(TResult)</c> if no element matches.
        /// </summary>
        /// <typeparam name="TSource">Element type of <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">Result type produced by <paramref name="selector"/>.</typeparam>
        /// <param name="source">The sequence to search.</param>
        /// <param name="predicate">Predicate used to find the first matching element.</param>
        /// <param name="selector">Projection applied to the matched element.</param>
        /// <returns>The projected value, or <c>default(TResult)</c> if no element matched.</returns>
        public static TResult FirstOrDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, bool> predicate, Func<TSource, TResult> selector)
        {
            var match = source.FirstOrDefault(predicate);
            return Equals(match, default(TSource)) ? default : selector(match);
        }
    }
}
