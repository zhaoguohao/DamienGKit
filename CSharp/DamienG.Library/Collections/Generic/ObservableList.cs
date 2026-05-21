// Copyright (c) Damien Guard.  All rights reserved.
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
// Originally published at http://damieng.com/blog/2006/06/14/observing_change_events_on_a_listt

using System;
using System.Collections;
using System.Collections.Generic;

namespace DamienG.Collections.Generic
{
    public class ObservableList<T> : IList<T>
    {
        private readonly IList<T> _internalList;

        public ObservableList()
        {
            _internalList = new List<T>();
        }

        public ObservableList(IList<T> list)
        {
            _internalList = list ?? throw new ArgumentNullException(nameof(list));
        }

        public ObservableList(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            _internalList = new List<T>(collection);
        }

        public int IndexOf(T item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _internalList.Insert(index, item);
            OnListChanged(new ListChangedEventArgs(index, item));
        }

        public void RemoveAt(int index)
        {
            var item = _internalList[index];
            _internalList.RemoveAt(index);
            OnListChanged(new ListChangedEventArgs(index, item));
        }

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

        public void Add(T item)
        {
            _internalList.Add(item);
            OnListChanged(new ListChangedEventArgs(_internalList.Count - 1, item));
        }

        public void Clear()
        {
            _internalList.Clear();
            OnListCleared(EventArgs.Empty);
        }

        public bool Contains(T item)
        {
            return _internalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _internalList.CopyTo(array, arrayIndex);
        }

        public int Count => _internalList.Count;

        public bool IsReadOnly => _internalList.IsReadOnly;

        public bool Remove(T item)
        {
            var index = _internalList.IndexOf(item);
            if (!_internalList.Remove(item))
                return false;

            OnListChanged(new ListChangedEventArgs(index, item));
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_internalList).GetEnumerator();
        }

        public event EventHandler<ListChangedEventArgs> ListChanged = delegate { };
        public event EventHandler ListCleared = delegate { };

        protected virtual void OnListChanged(ListChangedEventArgs e)
        {
            ListChanged(this, e);
        }

        protected virtual void OnListCleared(EventArgs e)
        {
            ListCleared(this, e);
        }

        public class ListChangedEventArgs : EventArgs
        {
            internal ListChangedEventArgs(int index, T item)
            {                
                Index = index;
                Item = item;
            }

            public int Index { get; }

            public T Item { get; }
        }
    }
}