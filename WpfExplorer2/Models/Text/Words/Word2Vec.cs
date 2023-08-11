using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.ML.WordModels
{
    public class Word2Vec
    {
        private List<decimal[]> _weights; //extend vocabulary with fixed dimensioned-vectors
        private int _dimension; //hyper-parameter: count of dimensions => size of vector.
        private int _size; // count of vectors.
        private decimal[] _emptyword;

        public Word2Vec(int dimension)
        {
            _dimension = dimension;
            _weights = new List<decimal[]>();
            _emptyword = new decimal[_dimension];
            _size = 0;
        }

        //return vector by index in list.
        public decimal[] this[int i] { 
            get { if (i < 0) { return _weights[_weights.Count + i]; }  return _weights[i]; }
            set { if (i > _size) i = _size; else if (i < 0) i = _size + i; _weights[i] = value; _size++; }
        }

        //Assign vector at i-row.
        public Word2Vec Add(int i, IEnumerable<decimal> value)
        {
            this[i] = value.ToArray();
            return this;
        }

        //Assign empty vector.
        public Word2Vec Add(int i)
        {
            this[i] = _emptyword;
            return this;
        }
    }
}
