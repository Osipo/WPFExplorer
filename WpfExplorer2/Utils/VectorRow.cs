using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorerControl.Utils
{
    public class VectorRow : IEnumerable<double>
    {
        double[] _data;
        int _size;
        public VectorRow(double[] data) {
            _data = data;
            _size = data.Length;
        }

        public VectorRow(int size)
        {
            _data = new double[size];
            _size = size;
        }

        public double[] Data { get { return _data; } }
        public int Size { get { return _size; } }

        public VectorRow Add(IEnumerable<double> B)
        {
            if (_size != B.Count())
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] += B.ElementAt(i);
            return this;
        }

        public VectorRow Sub(IEnumerable<double> B)
        {
            if (_size != B.Count())
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] -= B.ElementAt(i);
            return this;
        }

        public VectorRow Product(IEnumerable<double> B)
        {
            if (_size != B.Count())
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] *= B.ElementAt(i);
            return this;
        }

        public VectorRow Div(IEnumerable<double> B)
        {
            if (_size != B.Count())
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] /= B.ElementAt(i);
            return this;
        }

        public VectorRow Add(VectorRow B)
        {
            if (_size != B._size)
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] += B._data[i];
            return this;
        }

        public VectorRow Sub(VectorRow B)
        {
            if (_size != B._size)
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] -= B._data[i];
            return this;
        }

        public VectorRow Product(VectorRow B)
        {
            if (_size != B._size)
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] *= B._data[i];
            return this;
        }

        public VectorRow Div(VectorRow B)
        {
            if (_size != B._size)
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            for (int i = 0; i < _size; i++)
                _data[i] /= B._data[i];
            return this;
        }

        public VectorRow Mull(double a)
        {
            for (int i = 0; i < _data.Length; i++) _data[i] *= a;
            return this;
        }

        public VectorRow Add(double a)
        {
            for (int i = 0; i < _data.Length; i++) _data[i] += a;
            return this;
        }

        public VectorRow Sub(double a)
        {
            for (int i = 0; i < _data.Length; i++) _data[i] -= a;
            return this;
        }

        public VectorRow Div(double a)
        {
            for (int i = 0; i < _data.Length; i++) _data[i] /= a;
            return this;
        }

        public double Dot(IEnumerable<double> B)
        {
            if (_size != B.Count())
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            double S = 0.0;
            for(int i = 0; i < _size; i++)
            {
                S += (_data[i] * B.ElementAt(i));
            }
            return S;
        }

        public double Dot(VectorRow B)
        {
            if (_size != B._size)
                throw new ArgumentException("Argument Vector Y must be the same size as instance X");
            double S = 0.0;
            for (int i = 0; i < _size; i++)
            {
                S += (_data[i] * B._data[i]);
            }
            return S;
        }


        public double Norm()
        {
            double S = 0.0;
            for (int i = 0; i < _size; i++)
            {
                S += (_data[i] * _data[i]);
            }
            return Math.Sqrt(S);
        }

        public Matrix2D OuterProduct(IEnumerable<double> Y)
        {
            int XC = _data.Length, YC = Y.Count();

            double[][] result = new double[YC][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[XC];
            }
            for (int i = 0; i < XC; i++)
                for (int j = 0; j < YC; j++)
                    result[j][i] = _data[i] * Y.ElementAt(j);


            return new Matrix2D(result);
        }

        public static IEnumerable<double> Empty(int size, double v)
        {
            double[] e = new double[size];
            for (int i = 0; i < e.Length; i++) e[i] = v;
            return e;
        }

        public static IEnumerable<double> Empty(int size)
        {
            double[] e = new double[size];
            return e;
        }

        public IEnumerable<double> Mull(Matrix2D M)
        {
            double[][] mData = M.Data;
            double[] result = new double[M.Cols];
            for (int i = 0; i < M.Cols; i++)
            {
                for (int j = 0; j < M.Rows; j++)
                    result[i] += mData[j][i] * _data[i];
            }
            return result;
        }

        public IEnumerator<double> GetEnumerator()
        {
            return new VectorEnumerator(_data);
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }

    }

    public class VectorEnumerator : IEnumerator<double> {

        private double[] _data;
        private int _i;
        private bool _disposed;
        public VectorEnumerator(double[] data)
        {
            _data = data;
            _disposed = false;
            _i = -1;
        }

        public double Current { get { return _data[_i]; } }

        private object Current1 { get { return this.Current; } }

        object IEnumerator.Current { get { return this.Current1; } }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _data = null;
                _i = -1;
            }
            _disposed = true;
        }

        public bool MoveNext()
        {
            _i = _i + 1;
            return (_i >= 0 && _i < _data.Length);
        }

        public void Reset()
        {
            _i = -1;
        }
    }
}
