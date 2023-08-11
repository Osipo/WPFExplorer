using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorer.Models.ML.Networks
{
    public interface ICostFunction
    {
        string Name { get; }

        IEnumerable<double> Derivative(IEnumerable<double> expected, IEnumerable<double> predicted);
        double Total(IEnumerable<double> expected, IEnumerable<double> predicted); //expected => train, actual => predicted values.
    }

    // C = 1/n * ∑(y−exp)^2
    public class MSE : ICostFunction
    {
        public MSE() { }

        public string Name => nameof(MSE);


        // Mean Sum of squares 1/n * sum of (xi - yi)^2
        // actual => predicted.
        public double Total(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            IEnumerable<double> Sub = expected.Sub(predicted);
            return Sub.Dot<IEnumerable<double>>(Sub) / predicted.Count();
        }

        //Derivative of expression
        // 2/n * (xi - yi)
        public IEnumerable<double> Derivative(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            return predicted.Sub(expected).Mull(2.0 / predicted.Count()).ToList();
        }

    }

    //  C = ∑(y−exp)^2
    public class Quadratic : ICostFunction
    {
        public Quadratic() { }

        public string Name => nameof(Quadratic);


        // Sum of squares => sum of (xi - yi)^2
        public double Total(IEnumerable<double> expected, IEnumerable<double> predicted)
        {

            IEnumerable<double> Sub = predicted.Sub(expected);
            return Sub.Dot<IEnumerable<double>>(Sub);
        }

        //Derivative of expression
        // 2 * (xi - yi)
        public IEnumerable<double> Derivative(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            return predicted.Sub(expected).Mull(2.0).ToList();
        }

    }

    // C = 1/2 * ∑(y−exp)^2
    public class HalfQuadratic : ICostFunction
    {
        public HalfQuadratic() { }

        public string Name => nameof(HalfQuadratic);


        // Sum of squares mulltiplied by 1/2 => 1/2 * sum of (xi - yi)^2
        public double Total(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            IEnumerable<double> Sub = expected.Sub(predicted);
            return Sub.Dot<IEnumerable<double>>(Sub) * 0.5;
        }

        //Derivative of expression
        // (xi - yi)
        public IEnumerable<double> Derivative(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            return predicted.Sub(expected).ToList();
        }

    }

    // C =  -∑ (xi * log(yi)) + (1 - xi) * log(1 - yi) )
    public class CrossEntropy : ICostFunction
    {
        public CrossEntropy() { }

        public string Name => nameof(CrossEntropy);
        
        public double Total(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            return -expected.Zip(predicted, (xi, yi) => xi * Math.Log(yi) + (1 - xi) * Math.Log((1 - yi))).Sum();
        }

        // ∑(xi - yi) * xi
        public IEnumerable<double> Derivative(IEnumerable<double> expected, IEnumerable<double> predicted)
        {
            return expected.Sub(predicted).Product(predicted).ToList();
        }
    }
}
