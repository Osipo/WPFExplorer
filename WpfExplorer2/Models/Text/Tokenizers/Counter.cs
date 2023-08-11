using System;
using System.Collections.Generic;
using System.Linq;

namespace WpfExplorer.Models.ML
{
    public class Counter<T>
    {
        private IEnumerable<T> _li;
        private Dictionary<T, Int32> d;
        public Counter(IEnumerable<T> list)
        {
            _li = list;
            d = new Dictionary<T, Int32>();
            foreach (var Item in _li)
            {
                try
                {
                    d.Add(Item, 0);
                }
                catch (Exception e)
                {

                }
            }
        }

        private (T Elem, Int32 C) _MaxCount(T skeep)
        {
            foreach (var Item in _li)
            {
                if (!(d.ContainsKey(Item)))
                    continue;
                if (skeep != null && skeep.Equals(Item))
                {
                    d.Remove(Item);
                    continue;
                }
                d[Item] += 1;
            }
            Int32 k = d.Max(x => x.Value); //Max frequency.

            T Element = d.FirstOrDefault(x => x.Value == k).Key; //First Element with Max Frequency.
            return (Element, k);
        }


        private void _Flush()
        {
            foreach (var Item in _li)
            {
                d[Item] = 0;//HIDDEN:: adding new Key despite of Remove(k)...
            }
        }
        private void _Reset()
        {
            List<T> _kl = new List<T>(); //buffer.

            foreach (var Item in d.Keys)
            {
                _kl.Add(Item);
            }
            foreach (var Item in _kl)
            {
                d[Item] = 0;//set 0 and not add new key.
            }
        }

        /// <summary>
        /// Compute the list of N tuples that contains frequency for each element in Counter
        /// </summary>
        /// <param name="n">Count of tuples. For example, if two most common items are needed then n = 2.</param>
        /// <returns>List of tuples (Element, Frequency)</returns>
        public List<(T, Int32)> MostCommon(Int32 n)
        {
            if (n <= 0)
                n = 1;
            if (n > d.Count)
                n = d.Count;
            
            Int32 i = 0;
            List<(T, Int32)> li = new List<(T, Int32)>();
            T skip = default(T);

            while (i < n)
            {
                var v = _MaxCount(skip);
                li.Add(v);
                skip = v.Elem;
                i += 1;
                _Reset();
            }
            _Flush();
            return li;
        }


        /// <summary>
        /// Returns frequencies for each element in sequence of Counter.
        /// </summary>
        public List<Int32> Values()
        {
            List<(T e, Int32 c)> tuples = MostCommon(d.Count);
            List<Int32> v = new List<Int32>();
            foreach (var t in tuples)
            {
                v.Add(t.c);
            }
            return v;
        }

        public List<T> Keys()
        {
            List<(T e, Int32 c)> tuples = MostCommon(d.Count);
            List<T> v = new List<T>();
            foreach (var t in tuples)
            {
                v.Add(t.e);
            }
            return v;
        }

        public List<(T, Int32)> Frequencies()
        {
            return MostCommon(d.Count);
        }
    }
}
