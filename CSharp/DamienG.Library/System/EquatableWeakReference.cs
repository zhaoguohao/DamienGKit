// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2006/08/28/equatable_weak_references

using System;
using System.Runtime.InteropServices;

namespace DamienG.System
{
    /// <summary>
    /// Strongly-typed version of WeakReference that supports IEquatable too.
    /// </summary>
    /// <typeparam name="T">The reference type being weakly referenced.</typeparam>
    /// <remarks>
    /// The hash code is captured from the target at construction time and never re-read.
    /// If <typeparamref name="T"/> overrides <see cref="object.GetHashCode"/> based on mutable
    /// state, the cached value may become stale relative to a current call on the live target.
    /// </remarks>
    public sealed class EquatableWeakReference<T> : IEquatable<EquatableWeakReference<T>>, IDisposable where T : class
    {
        private readonly int _hashCode;
        private GCHandle _handle;

        /// <summary>
        /// Create a new <see cref="EquatableWeakReference{T}"/> that weakly references <paramref name="target"/>.
        /// </summary>
        /// <param name="target">The object to weakly reference. Must not be null.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="target"/> is null.</exception>
        public EquatableWeakReference(T target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            _hashCode = target.GetHashCode();
            _handle = GCHandle.Alloc(target, GCHandleType.Weak);
        }

        /// <summary>True while the target has not yet been garbage-collected.</summary>
        public bool IsAlive => _handle.Target != null;

        /// <summary>The referenced object, or <c>null</c> if it has been garbage-collected.</summary>
        public T Target => _handle.Target as T;

        /// <inheritdoc/>
        public void Dispose()
        {
            if (_handle.IsAllocated)
                _handle.Free();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc/>
        public bool Equals(EquatableWeakReference<T> other)
        {
            return other != null && ReferenceEquals(other.Target, Target);
        }

        /// <summary>Finalizer that frees the underlying <see cref="GCHandle"/> if <see cref="Dispose"/> was not called.</summary>
        ~EquatableWeakReference()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return obj is EquatableWeakReference<T> reference && Equals(reference);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return _hashCode;
        }
    }
}
