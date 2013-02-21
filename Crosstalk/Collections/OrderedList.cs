using System;
using System.Collections;
using System.Collections.Generic;

namespace Crosstalk.Core.Collections
{
    public class OrderedList<T>:IList<T>
    {
        private readonly Func<T, T, int> _comparer;
        private readonly List<T> _internal;

        public OrderedList(Func<T, T, int> comparer)
        {
            this._comparer = comparer;
            this._internal = new List<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this._internal.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public void AddRange(IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                this.Add(item);
            }
        }

        public void Add(T item)
        {
            int start = 0, end = this._internal.Count, target = 0;
            while (start != end)
            {
                target = (int) Math.Floor((decimal) (end - start)/2) + start;
                var sort = this._comparer(this._internal[target], item);
                if (sort > 0)
                {
                    target += start == target ? 1 : 0;
                    start = target;
                } else if (sort < 0)
                {
                    end = target;
                }
                else
                {
                    break;
                }
            }
            this._internal.Insert(target, item);
        }

        public void Clear()
        {
            this._internal.Clear();
        }

        public bool Contains(T item)
        {
            return this._internal.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this._internal.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return this._internal.Remove(item);
        }

        public int Count { get; private set; }

        public bool IsReadOnly { get; private set; }

        public int IndexOf(T item)
        {
            return this._internal.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            this.Add(item);
        }

        public void RemoveAt(int index)
        {
            this._internal.RemoveAt(index);
        }

        public T this[int index]
        {
            get { return this._internal[index]; }
            set { this.Add(value); }
        }
    }
}