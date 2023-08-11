using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Networks.Activations;

namespace WpfExplorer.Models
{
    /* Global objects */
    public class StaticObjects
    {
        public static readonly object True = true;
        public static readonly object False = false;
        public static readonly object Reciever = new ViewModels.ViewModelBaseWithGuid();
        public static readonly object Lock = new object();
        public static readonly char[] EcmaSeparators = new char[] { ' ', '\n', '\r', '\t', '\v', '\f' };
        public static readonly char[] Space = new char[] { ' ' };

        //Activation Objects.
        public static readonly IActivation<decimal> MReLu = new DecimalActivation("ReLu", (x) => x <= 0 ? 0 : x,
            (x) => x <= 0 ? 0 : 1M);
        public static readonly IActivation<double> ReLu = new DoubleActivation("ReLu", (x) => x <= 0 ? 0 : x,
            (x) => x <= 0 ? 0 : 1D);

        public static readonly IActivation<decimal> MLeakyReLu = new DecimalActivation("LeakyReLu", (x) => x <= 0 ? 0.01m * x : x,
            (x) => x <= 0 ? 0.01M : 1M);
        public static readonly IActivation<double> LeakyReLu = new DoubleActivation("LeakyReLu", (x) => x <= 0 ? 0.01 * x : x,
            (x) => x <= 0 ? 0.01D : 1D);

        public static readonly IActivation<decimal> MIdentity = new DecimalActivation("Identity", (x) => x, x => 1M);
        public static readonly IActivation<double> Identity = new DoubleActivation("Identity", (x) => x, x => 1D);


        public static readonly IActivation<double> Sigmoid = new DoubleActivation("Sigmoid", BaseActivation<double>.Sigmoid,
            (x) => BaseActivation<double>.Sigmoid(x) * (1.0 - BaseActivation<double>.Sigmoid(x)));

        public static readonly IActivation<double> SoftPlus = new DoubleActivation("SoftPlus", (x) => Math.Log(1.0 + Math.Exp(x)),
            (x) => BaseActivation<double>.Sigmoid(x));

        private static readonly IActivation<double> SoftMax = new SoftMaxActivation();
    }
}
