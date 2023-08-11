using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Networks.Optimizers;
using WpfExplorerControl.Extensions;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks
{
    public class NeuralNetwork
    {
        private readonly ICostFunction _costFunction;
        private readonly int _inputSize;
        private readonly double _l2;
        private IOptimizer _optimizer;
        private readonly List<Layer> _layers;
        public NeuralNetwork(List<Layer> layers, int inputSize, ICostFunction costF, IOptimizer optimizer, Initializer initializer, double l2) { 
            _layers = new List<Layer>();
            _inputSize = inputSize;
            _costFunction = costF;
            _optimizer = optimizer;
            _l2 = l2;


            Layer inputLayer = new Layer(inputSize, StaticObjects.Identity);
            _layers.Add(inputLayer);

            Layer prev = inputLayer;
            for(int i = 0; i < layers.Count; i++)
            {
                Layer li = layers[i];
                Matrix2D W = new Matrix2D(prev.Size, li.Size);
                initializer.Invoke(W, i);
                li.Weights = W;
                li.Optimizer = optimizer; //stateless optimizer for all layers.
                li.L2 = l2;
                li.PrecedingLayer = prev;
                _layers.Add(li);

                prev = li;
            }
        }

        public IEnumerable<double> Eval(IEnumerable<double> input)
        {
            return Eval(input, null);
        }
        public IEnumerable<double> Eval(IEnumerable<double> input, IEnumerable<double> expected)
        {
            IEnumerable<double> signal = input;
            foreach(var layer in _layers)
            {
                signal = layer.Eval(signal);
            }

            if(expected != null)
            {
                LearnFrom(expected);
                double cost = _costFunction.Total(expected, signal);
                return signal.Union(new[] { cost }, new IgnoreEqualityComparer<double>()); 
            }
            return signal;
        }

        private void LearnFrom(IEnumerable<double> expected)
        {
            Layer l = _layers.Last();
            IEnumerable<double> dCdO = _costFunction.Derivative(expected, l.GetOut());
            //Back propagate to input layer.
            do
            {
                IEnumerable<double> dCdI = l.Activation.dCdI(l.GetOut(), dCdO); //dCdO * dCdW
                Matrix2D dCdW = dCdI.OuterProduct(l.PrecedingLayer.GetOut());

                l.AddDeltaWeightsAndBiases(dCdW, dCdI);
                dCdO = l.Weights.Mull(dCdI);
                l = l.PrecedingLayer;
            } while (l.HasPrecedingLayer);
        }

        public void UpdateFromLearning()
        {
            
            foreach (var layer in _layers)
                if (layer.HasPrecedingLayer)
                    layer.UpdateWeightsAndBias();
        }

        public int InputSize { get { return _inputSize;} }
        public IOptimizer Optimizer { get { return _optimizer; } }
        public ICostFunction CostFunction { get { return _costFunction;} }
        public double L2 { get { return _l2; } }

        public List<Layer> Layers { get { return _layers; } }
    }
}
