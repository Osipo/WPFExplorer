using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorerControl.Utils
{
    public class LIFOComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return -1; //each element preceeds previous => LIFO Last in is First Out
        }
    }
}
