using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Networks.Optimizers;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks
{
    public class NetworkBuilder
    {
        private readonly List<Layer> _layers;
        private readonly int _networkInputSize;

        // defaults:
        private Initializer _initializer = WeightInitializers.XavierNormal; //new Initializer.Random(-0.5, 0.5);
        private ICostFunction _costFunction = new Quadratic();
        private IOptimizer _optimizer = new GradientDescent(0.005);
        private double _l2 = 0.0;

        public NetworkBuilder(int networkInputSize)
        {
            _networkInputSize = networkInputSize;
            _layers = new List<Layer>();
        }

        /**
         * Create a builder from an existing neural network, hence making
         * it possible to do a copy of the entire state and modify as needed.
         */
        public NetworkBuilder(NeuralNetwork other)
        {
            _networkInputSize = other.InputSize;
            _costFunction = other.CostFunction;
            _optimizer = other.Optimizer;
            _l2 = other.L2;

            List<Layer> otherLayers = other.Layers;
            int l = otherLayers.Count;
            for (int i = 1; i < l; i++)
            {
                Layer otherLayer = otherLayers.ElementAt(i);
                _layers.Add(
                    new Layer(
                        otherLayer.Size,
                        otherLayer.Activation,
                        otherLayer.Bias
                    )
                );
            }

            _initializer = (weights, layer) => {
                Layer otherLayer = otherLayers.ElementAt(layer + 1);
                Matrix2D otherLayerWeights = otherLayer.Weights;
                weights.FillFrom(otherLayerWeights); //just copy values from other to weights.
            };
        }

        public NetworkBuilder SetInitializer(Initializer initializer)
        {
            _initializer = initializer;
            return this;
        }

        public NetworkBuilder SetCostFunction(ICostFunction costFunction)
        {
            this._costFunction = costFunction;
            return this;
        }

        public NetworkBuilder SetOptimizer(IOptimizer optimizer)
        {
            this._optimizer = optimizer;
            return this;
        }

        public NetworkBuilder L2(double l2)
        {
            this._l2 = l2;
            return this;
        }

        public NetworkBuilder AddLayer(Layer layer)
        {
            _layers.Add(layer);
            return this;
        }

        public NeuralNetwork create()
        {
            return new NeuralNetwork(_layers, _networkInputSize, _costFunction, _optimizer, _initializer, _l2);
        }

        
    }
}
