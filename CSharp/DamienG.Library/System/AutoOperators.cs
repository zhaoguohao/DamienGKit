// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2005/10/11/automaticcomparisonoperatoroverloadingincsharp

using System;

namespace DamienG.System
{
    /// <summary>
    /// A base class that automatically provides all operator overloads based on your class
    /// only implementing CompareTo.
    /// </summary>
    /// <remarks>
    /// Equality is derived from ordering: <see cref="op_Equality"/> returns true whenever
    /// <see cref="CompareTo"/> returns zero. Subclasses that need a notion of equality
    /// distinct from ordering should not derive from this class.
    /// </remarks>
    public abstract class AutoOperators : IComparable
    {
        /// <inheritdoc/>
        public abstract int CompareTo(object obj);

        /// <summary>Returns true when <paramref name="obj1"/> is ordered before <paramref name="obj2"/>.</summary>
        public static bool operator <(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) < 0;
        }

        /// <summary>Returns true when <paramref name="obj1"/> is ordered after <paramref name="obj2"/>.</summary>
        public static bool operator >(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) > 0;
        }

        /// <summary>Returns true when <paramref name="obj1"/> and <paramref name="obj2"/> compare equal.</summary>
        public static bool operator ==(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) == 0;
        }

        /// <summary>Returns true when <paramref name="obj1"/> and <paramref name="obj2"/> do not compare equal.</summary>
        public static bool operator !=(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) != 0;
        }

        /// <summary>Returns true when <paramref name="obj1"/> is ordered before or equal to <paramref name="obj2"/>.</summary>
        public static bool operator <=(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) <= 0;
        }

        /// <summary>Returns true when <paramref name="obj1"/> is ordered after or equal to <paramref name="obj2"/>.</summary>
        public static bool operator >=(AutoOperators obj1, AutoOperators obj2)
        {
            return Compare(obj1, obj2) >= 0;
        }

        /// <summary>
        /// Null-safe comparison that delegates to <see cref="CompareTo"/> when both operands are non-null.
        /// </summary>
        /// <param name="obj1">The first operand.</param>
        /// <param name="obj2">The second operand.</param>
        /// <returns>
        /// 0 if both are the same reference or both null; -1 if only <paramref name="obj1"/> is null;
        /// 1 if only <paramref name="obj2"/> is null; otherwise the result of <c>obj1.CompareTo(obj2)</c>.
        /// </returns>
        public static int Compare(AutoOperators obj1, AutoOperators obj2)
        {
            if (ReferenceEquals(obj1, obj2))
                return 0;
            if (obj1 is null)
                return -1;
            if (obj2 is null)
                return 1;
            return obj1.CompareTo(obj2);
        }

        /// <inheritdoc/>
        public abstract override int GetHashCode();

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is AutoOperators operators))
                return false;
            return this == operators;
        }
    }
}
