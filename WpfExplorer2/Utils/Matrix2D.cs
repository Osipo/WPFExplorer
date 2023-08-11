using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorerControl.Extensions;

namespace WpfExplorerControl.Utils
{
    public class Matrix2D : IEnumerable<IEnumerable<double>>
    {
        private double[][] _data;

        private int _rows;
        private int _columns;

        public Matrix2D(int rows, int columns)
        {
            _rows = rows;
            _columns = columns;
            _data = new double[_rows][];
            for (int i = 0; i < _rows; i++)
                _data[i] = new double[_columns];
        }

        public Matrix2D(double[][] data)
        {
            _data = data;
            _rows = data.GetLength(0);
            _columns = data.AsEnumerable().Max(x => x.Length);
        }

        public Matrix2D(Matrix2D M)
        {
            _rows = M.Rows;
            _columns = M.Cols;
            _data = new double[M.Rows][];
            for(int i = 0; i < _rows; i++)
            {
                _data[i] = new double[_columns];
                Array.Copy(M.Data[i], 0, _data[i], 0, _columns);
            }
        }

        public double[][] Data { get { return _data; } }

        public double this[int row, int col]
        {
            get { return _data[row][col]; }
            set { _data[row][col] = value; }
        }

        public int Rows { get { return _rows; } }

        public int Cols { get { return _columns; } }


        #region ChangeMatrix
        public Matrix2D Apply(Func<double, double> f)
        {
            for(int i = 0; i < _rows; i++)
            for(int j = 0; j < _columns; j++)
                this[i, j] = f(this[i, j]);
            return this;
        }

        public Matrix2D ScalarMul(double c)
        {
            return Apply((x) => x * c);
        }

        public Matrix2D ScalarAdd(double c)
        {
            return Apply((x) => x + c);
        }

        public Matrix2D SubFrom(Matrix2D B)
        {
            if (B._rows != _rows || B._columns != _columns)
                throw new ArgumentException("Addition is allowed for only matrixes of the identical size.", "B");
            for (Int32 i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    this[i, j] -= B[i, j];
            return this;
        }

        public Matrix2D AddTo(Matrix2D B)
        {
            if (B._rows != _rows || B._columns != _columns)
                throw new ArgumentException("Addition is allowed for only matrixes of the identical size.", "B");
            for (Int32 i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    this[i, j] += B[i, j];
            return this;
        }

        public Matrix2D FillFrom(Matrix2D B)
        {
            if (B._rows != _rows || B._columns != _columns)
                throw new ArgumentException("Addition is allowed for only matrixes of the identical size.", "B");
            for (Int32 i = 0; i < _rows; i++)
                for (int j = 0; j < _columns; j++)
                    this[i, j] = B[i, j];
            return this;
        }

        public IEnumerable<double> Mull(IEnumerable<double> V)
        {
            double[] r = new double[_rows];
            for (int i = 0; i < _rows; i++)
                r[i] = _data[i].Dot<IEnumerable<double>>(V);
            return r;
        }

        public Matrix2D Transpose()
        {
            Double[][] ndata = new Double[_columns][];
            for (Int32 i = 0; i < _columns; i++)
            {
                ndata[i] = new double[_rows];
                for (int j = 0; j < _rows; j++)
                    ndata[i][j] = _data[j][i];
            }
            _data = ndata;
            int t = _rows;
            _rows = _columns;
            _columns = t;
            return this;
        }

        #endregion

        #region IEnumerable
        public IEnumerator<IEnumerable<double>> GetEnumerator()
        {
            return new MatrixEnumerator(_data);
        }

        private IEnumerator GetEnumerator1()
        {
            return this.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator1();
        }
        #endregion
    }

    public class MatrixEnumerator : IEnumerator<IEnumerable<double>>
    {
        private double[][] _data;
        private int _i;
        private bool _disposed;
        public MatrixEnumerator(double[][] data)
        {
            _data = data;
            _i = -1;
            _disposed = false;
        }

        //Return Current i-th Column (Vector) of Matrix
        public IEnumerable<double> Current { get { return _data.Select(x => x[_i]); } }

        private object Current1
        {
            get { return this.Current; }
        }

        object IEnumerator.Current
        {
            get { return this.Current1; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(_disposed) return;
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
            return(_i >= 0 && _i < _data.Length);
        }

        public void Reset()
        {
            _i = -1;
        }
    }
}
