// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License.
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2006/06/14/observing_change_events_on_a_listt

using System;
using System.Collections;
using System.Collections.Generic;

namespace DamienG.Collections.Generic
{
    /// <summary>
    /// An <see cref="IList{T}"/> that raises events when items are added, removed, replaced, or cleared.
    /// </summary>
    /// <typeparam name="T">Element type.</typeparam>
    public class ObservableList<T> : IList<T>
    {
        private readonly IList<T> _internalList;

        /// <summary>Create an empty <see cref="ObservableList{T}"/>.</summary>
        public ObservableList()
        {
            _internalList = new List<T>();
        }

        /// <summary>Create an <see cref="ObservableList{T}"/> that wraps and observes <paramref name="list"/>.</summary>
        /// <param name="list">The list to wrap. Subsequent operations through this <see cref="ObservableList{T}"/> mutate <paramref name="list"/> directly.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="list"/> is null.</exception>
        public ObservableList(IList<T> list)
        {
            _internalList = list ?? throw new ArgumentNullException(nameof(list));
        }

        /// <summary>Create an <see cref="ObservableList{T}"/> initially populated with a copy of <paramref name="collection"/>.</summary>
        /// <exception cref="ArgumentNullException">If <paramref name="collection"/> is null.</exception>
        public ObservableList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            _internalList = new List<T>(collection);
        }

        /// <inheritdoc/>
        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListChanged"/>.</remarks>
        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            OnListChanged(new ListChangedEventArgs(index, item));
        }

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListChanged"/>.</remarks>
        public void RemoveAt(int index)
        {
            var item = _internalList[index];
            _internalList.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(index, item));
        }

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListChanged"/> on the setter only when the new value differs from the current one.</remarks>
        public T this[int index]
        {
            get => _internalList[index];
            set
            {
                if (EqualityComparer<T>.Default.Equals(_internalList[index], value)) return;

                _internalList[index] = value;
                OnListChanged(new ListChangedEventArgs(index, value));
            }
        }

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListChanged"/>.</remarks>
        public void Add(T item)
        {
            _internalList.Add(item);
            OnListChanged(new ListChangedEventArgs(_internalList.Count - 1, item));
        }

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListCleared"/>.</remarks>
        public void Clear()
        {
            _internalList.Clear();
            OnListCleared(EventArgs.Empty);
        }

        /// <inheritdoc/>
        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        /// <inheritdoc/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public int Count => _internalList.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => _internalList.IsReadOnly;

        /// <inheritdoc/>
        /// <remarks>Raises <see cref="ListChanged"/> when an item was actually removed.</remarks>
        public bool Remove(T item)
        {
            var index = _internalList.IndexOf(item);
            if (!_internalList.Remove(item))
                return false;

            OnListChanged(new ListChangedEventArgs(index, item));
            return true;
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_internalList).GetEnumerator();
        }

        /// <summary>Raised when an item is added, inserted, removed, or replaced.</summary>
        public event EventHandler<ListChangedEventArgs> ListChanged = delegate { };

        /// <summary>Raised when the list is cleared via <see cref="Clear"/>.</summary>
        public event EventHandler ListCleared = delegate { };

        /// <summary>Raises <see cref="ListChanged"/>.</summary>
        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged(this, e);
        }

        /// <summary>Raises <see cref="ListCleared"/>.</summary>
        protected virtual void OnListCleared(EventArgs e)
        {
            ListCleared(this, e);
        }

        /// <summary>Event data describing a single-item change to an <see cref="ObservableList{T}"/>.</summary>
        public class ListChangedEventArgs : EventArgs
        {
            internal ListChangedEventArgs(int index, T item)
            {
                Index = index;
                Item = item;
            }

            /// <summary>The index of the affected item within the list.</summary>
            public int Index { get; }

            /// <summary>The item that was added, removed, or set.</summary>
            public T Item { get; }
        }
    }
}
