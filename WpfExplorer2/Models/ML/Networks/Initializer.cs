using DevExpress.Mvvm.UI.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks
{
    public delegate void Initializer(Matrix2D W, int layer);
    public static class WeightInitializers {
        public static Random rnd = new Random();


        public static Initializer XavierUniform = (W, layer) =>
        {
            double factor = 2.0 * Math.Sqrt(6.0 / (W.Cols + W.Rows));
            W.Apply(x => (rnd.NextDouble() - 0.5) * factor);
        };

        public static Initializer XavierNormal = (W, layer) =>
        {
            double factor = Math.Sqrt(2.0 / (W.Cols + W.Rows));
            W.Apply(x => rnd.NextGaussian() * factor);
        };


    }
}
