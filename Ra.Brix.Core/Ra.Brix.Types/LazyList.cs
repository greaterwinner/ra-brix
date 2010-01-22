/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections;

namespace Ra.Brix.Types
{
    public delegate IEnumerable FunctorGetItems();

    public class LazyList<T> : IList<T>
    {
        private List<T> _list;
        private readonly FunctorGetItems _functor;
        private bool _listRetrieved;

        public LazyList(FunctorGetItems functor)
        {
            _functor = functor;
        }

        public LazyList()
        { }

        private void FillList()
        {
            if (_listRetrieved)
                return;
            _list = new List<T>();
            _listRetrieved = true;
            if (_functor == null)
                return;
            foreach (object tmp in _functor())
            {
                _list.Add((T)tmp);
            }
        }

        public bool ListRetrieved
        {
            get { return _listRetrieved; }
        }

        public void Sort(Comparison<T> functor)
        {
            FillList();
            _list.Sort(functor);
        }

        public bool Exists(Predicate<T> functor)
        {
            FillList();
            foreach (T idx in _list)
            {
                if (functor(idx))
                    return true;
            }
            return false;
        }

        public T Find(Predicate<T> functor)
        {
            FillList();
            foreach (T idx in _list)
            {
                if (functor(idx))
                    return idx;
            }
            return default(T);
        }

        public void AddRange(IEnumerable<T> collection)
        {
            FillList();
            _list.AddRange(collection);
        }

        public void RemoveAll(Predicate<T> functor)
        {
            List<T> toBeRemoved = new List<T>();
            foreach (T idx in this)
            {
                if (functor(idx))
                {
                    toBeRemoved.Add(idx);
                }
            }
            foreach (T idx in toBeRemoved)
            {
                Remove(idx);
            }
        }

        public int IndexOf(T item)
        {
            FillList();
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            FillList();
            _list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            FillList();
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                FillList();
                return _list[index];
            }
            set
            {
                FillList();
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            FillList();
            _list.Add(item);
        }

        public void Clear()
        {
            FillList();
            _list.Clear();
        }

        public bool Contains(T item)
        {
            FillList();
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            FillList();
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                FillList();
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            FillList();
            return _list.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            FillList();
            return _list.GetEnumerator();
        }
    }
}
