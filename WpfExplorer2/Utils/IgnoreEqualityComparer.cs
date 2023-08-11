using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorerControl.Utils
{
    public class IgnoreEqualityComparer<T> : IEqualityComparer<T>
    {
        //Always false.
        public bool Equals(T x, T y)
        {
            return false;
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
