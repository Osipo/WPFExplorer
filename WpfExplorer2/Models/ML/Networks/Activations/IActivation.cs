using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.Networks.Activations
{
    public interface IActivation<T>
    {
        string Name { get; }

        IEnumerable<T> Fn(IEnumerable<T> input);
        IEnumerable<T> dFn(IEnumerable<T> input);
        IEnumerable<T> dCdI(IEnumerable<T> input, IEnumerable<T> dCdO);
    }
}
