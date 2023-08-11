using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Models.ML.Networks.Activations
{
    public abstract class BaseActivation<T> : IActivation<T>
    {
        private readonly string _name;
        private Func<T, T> _fn;
        private Func<T, T> _dfn;
        public BaseActivation(string name, Func<T, T> function, Func<T, T> derivation) {
            _name = name;
            _fn = function;
            _dfn = derivation;
        }

        public string Name { get { return _name; } }

        public virtual IEnumerable<T> Fn(IEnumerable<T> input)
        {
            return input.Select(x => _fn(x)).ToList();
        }
        public virtual IEnumerable<T> dFn(IEnumerable<T> input)
        {
            return input.Select(x => _dfn(x)).ToList();
        }
        public abstract IEnumerable<T> dCdI(IEnumerable<T> input, IEnumerable<T> dCdO);

        public static double Sigmoid(double v)
        {
            return 1.0 / (1.0 + Math.Exp(-v));
        }
    }

}
