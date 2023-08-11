using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks.Optimizers
{
    public interface IOptimizer
    {
        IEnumerable<double> updateBias(IEnumerable<double> bias, IEnumerable<double> dCdBias);
        Matrix2D updateWeights(Matrix2D weights, Matrix2D dCdW);
    }
}
