using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks.Optimizers
{

    //Updates Weights and Bias by formula
    // W = W - lr * dW
    // B = B - lr * dB
    public class GradientDescent : IOptimizer
    {
        private double _learningRate;

        public GradientDescent(double learningRate)
        {
            _learningRate = learningRate;
        }
        public IEnumerable<double> updateBias(IEnumerable<double> bias, IEnumerable<double> dCdBias)
        {
            return bias.Sub(dCdBias.Mull(_learningRate));
        }

        public Matrix2D updateWeights(Matrix2D weights, Matrix2D dCdW)
        {
            return weights.SubFrom(dCdW.ScalarMul(_learningRate));
        }
    }
}
