using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Models.ML.Networks.Activations
{
    public class DecimalActivation : BaseActivation<decimal>, IActivation<decimal>
    {
        public DecimalActivation(string name, Func<decimal, decimal> function, Func<decimal, decimal> derivation) : base(name, function, derivation) { }

        public override IEnumerable<decimal> dCdI(IEnumerable<decimal> input, IEnumerable<decimal> dCdO)
        {
            return dCdO.Product(dFn(input)); //layer-by-layer.
        }
    }
}