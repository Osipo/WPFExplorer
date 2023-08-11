using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorerControl.Utils
{
    public class FIFOComparer<T> : IComparer<T>
    {
        public int Compare(T x, T y)
        {
            return 1; //each elements follows to previous.
        }
    }
}
