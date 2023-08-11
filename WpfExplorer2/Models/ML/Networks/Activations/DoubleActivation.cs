using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Models.ML.Networks.Activations
{
    public class DoubleActivation : BaseActivation<double>, IActivation<double>
    {
        public DoubleActivation(string name, Func<double, double> function, Func<double, double> derivation) : base(name, function, derivation)
        {
        }

        public override IEnumerable<double> dCdI(IEnumerable<double> input, IEnumerable<double> dCdO)
        {
            //Console.WriteLine(input.ToString<double>() + " x " + dCdO.ToString<double>());
            return dCdO.Product(dFn(input)); //layer-by-layer.
        }
    }
}
