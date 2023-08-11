using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Models.ML.Networks.Activations
{
    public class SoftMaxActivation : BaseActivation<double>, IActivation<double>
    {
        public SoftMaxActivation() : base("SoftMax", null, null)
        {

        }


        public override IEnumerable<double> Fn(IEnumerable<double> input)
        {
            double Max = input.MaxElem();
            double S = input.Aggregate(default(double), (acc, x) => acc + Math.Exp(x - Max));
            return input.Select(x => Math.Exp(x - Max) / S).ToList();
        }

        public override IEnumerable<double> dCdI(IEnumerable<double> input, IEnumerable<double> dCdO)
        {
            double S = input.Product(dCdO).Sum();
            IEnumerable<double> Sub = dCdO.Sub(S); //dCdO - Sum
            return input.Product(Sub).ToList();
        }
    }
}
