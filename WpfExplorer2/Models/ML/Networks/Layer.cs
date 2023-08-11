using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfExplorer.Models.ML.Networks.Activations;
using WpfExplorer.Models.ML.Networks.Optimizers;
using WpfExplorerControl.Extensions;
using WpfExplorerControl.Utils;

namespace WpfExplorer.Models.ML.Networks
{
    public class Layer
    {
        private readonly int _size;

        //private readonly ThreadLocal<IEnumerable<double>> _out = new ThreadLocal<IEnumerable<double>>();
        private IEnumerable<double> _out2;

        private readonly IActivation<double> _activation;
        private IOptimizer _optimizer;
        private Layer _precedingLayer;
        private double _l2 = 0;


        private Matrix2D _weights;
        private IEnumerable<double> _bias;
       

        // changes to the weights and biases ("observed things not yet learned")
        private Matrix2D _deltaWeights;
        private IEnumerable<double> _deltaBias;
        private int _deltaWeightsAdded = 0;
        private int _deltaBiasAdded = 0;

        public Layer(int size, IActivation<double> activation) : this(size, activation, 0)
        {
        }

        public Layer(int size, IActivation<double> activation, double initialBias)
        {
            _size = size;
            _bias = Enumerable.Empty<double>().EmptyArray(size, initialBias);
            _deltaBias = Enumerable.Empty<double>().EmptyArray(size, default(double));
            _activation = activation;
        }

        public Layer(int size, IActivation<double> activation, IEnumerable<double> bias)
        {
            _size = size;
            _bias = bias;
            _deltaBias = Enumerable.Empty<double>().EmptyArray(size, default(double));
            _activation = activation;
        }

        public int Size { get { return _size; } }

        public IActivation<double> Activation { get { return _activation; } }

        public Layer PrecedingLayer { get { return _precedingLayer; } set { _precedingLayer = value; } }

        public bool HasPrecedingLayer { get { return _precedingLayer != null; } }

        /**
         * Feed the in-vector, i, through this layer.
         * Stores a copy of the out vector.
         *
         * @param i The input vector
         * @return The out vector o (i.e. the result of o = iW + b)
         */
        public IEnumerable<double> Eval(IEnumerable<double> i)
        {
            if (!HasPrecedingLayer)
            {
                //_out.Value = i;
                _out2 = i;
            }
            else
            {
                //_out.Value = _activation.Fn(i.Mul(_weights).Add(_bias));
                _out2 = _activation.Fn(i.Mull(_weights).Add(_bias));
            }
            //return _out.Value;
            return _out2;
        }

        public IEnumerable<double> GetOut()
        {
            return _out2;//return _out.Value;
        }

        public Matrix2D Weights { get { return _weights; } set { _weights = value; _deltaWeights = new Matrix2D(_weights.Data); } }
        public IOptimizer Optimizer { get { return _optimizer; } set { _optimizer = value; } }

        public double L2 { get { return _l2; } set { _l2 = value; } }

        public IEnumerable<double> Bias { get { return _bias; } }

        /**
         * Add upcoming changes to the Weights and Biases.
         * This does not mean that the network is updated.
         */
        public void AddDeltaWeightsAndBiases(Matrix2D dW, IEnumerable<double> dB)
        {
            //lock (StaticObjects.Lock)
            //{
                _deltaWeights.AddTo(dW); //changes Matrix2D.
                _deltaWeightsAdded++;
                _deltaBias = _deltaBias.Add(dB);
                _deltaBiasAdded++;
            //}
        }

        /**
         * Takes an average of all added Weights and Biases and tell the
         * optimizer to apply them to the current weights and biases.
         * <p>
         * Also applies L2 regularization on the weights if used.
         */
        public void UpdateWeightsAndBias()
        {
            //lock (StaticObjects.Lock)
            //{
                if (_deltaWeightsAdded > 0)
                {
                    if (_l2 > 0)
                        _weights.Apply(value => value - _l2 * value);

                    Matrix2D average_dW = _deltaWeights.ScalarMul(1.0 / _deltaWeightsAdded);
                    _optimizer.updateWeights(_weights, average_dW);

                    //Clear Delta.
                    _deltaWeights.Apply(x => 0.0);
                    _deltaWeightsAdded = 0;
                }

                if (_deltaBiasAdded > 0)
                {
                    IEnumerable<double> average_bias = _deltaBias.Mull(1.0 / _deltaBiasAdded);
                    _bias = _optimizer.updateBias(_bias, average_bias);

                    //Clear Delta.
                    _deltaBias = _deltaBias.Select(x => 0.0);
                    _deltaBiasAdded = 0;
                }
            //}
        }
    }
}