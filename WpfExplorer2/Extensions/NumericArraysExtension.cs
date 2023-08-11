using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using WpfExplorerControl.Utils;
namespace WpfExplorerControl.Extensions
{
    public static class NumericArraysExtension
    {
        /// <summary>
        /// Scalar production of two vectors X and Y.
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <param name="Y">Vector Y</param>
        /// <returns>scalar production = Sum of xi * yi for i = 1..n</returns>
        /// <exception cref="ArgumentException">Size of vectors are diffferent or Vector Y was not specified.</exception>
        public static decimal Dot(this IEnumerable<decimal> X, IEnumerable<decimal> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a * b).Sum();
        }

        /// <summary>
        /// Scalar production of two vectors X and Y.
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <param name="Y">Vector Y</param>
        /// <returns>scalar production = Sum of xi * yi for i = 1..n</returns>
        /// <exception cref="ArgumentException">Size of vectors are diffferent or Vector Y was not specified.</exception>
        public static double Dot<R>(this R X, R Y) where R : IEnumerable<double> 
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a * b).Sum();
        }

        /// <summary>
        /// Computes Norm of the vector. This is the square root of scalar production of the vector X = sqrt(dot(X, X))
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <param name="eps">Tolerance of sqrt operation for decimal type</param>
        /// <returns>decimal number that is the norm of the vector X</returns>
        public static decimal Norm(this IEnumerable<decimal> X, decimal eps)
        {
            return X.Dot(X).Sqrt(eps);
        }

        /// <summary>
        /// Computes Norm of the vector. This is the square root of scalar production of the vector X = sqrt(dot(X, X))
        /// with tolerance 10E-13
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>decimal number that is the norm of the vector X</returns>
        public static decimal Norm(this IEnumerable<decimal> X)
        {
            return X.Dot(X).Sqrt(10E-13M);
        }

        /// <summary>
        /// Computes Norm of the vector. This is the square root of scalar production of the vector X = sqrt(dot(X, X))
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>double number that is the norm of the vector X</returns>
        public static double Norm(this IEnumerable<double> X)
        {
            return Math.Sqrt(X.Dot<IEnumerable<double>>(X));
        }

        public static IEnumerable<decimal> Mull(this IEnumerable<decimal> X, decimal a)
        {
            return X.Select(x => x * a).ToList();
        }

        public static IEnumerable<double> Mull(this IEnumerable<double> X, double a)
        {
            return X.Select(x => x * a).ToList();
        }

        public static IEnumerable<decimal> Sub(this IEnumerable<decimal> X, decimal a)
        {
            return X.Select(x => x - a).ToList();
        }

        public static IEnumerable<double> Sub(this IEnumerable<double> X, double a)
        {
            return X.Select(x => x - a).ToList();
        }


        #region Arithmetic
        public static IEnumerable<decimal> Product(this IEnumerable<decimal> X, IEnumerable<decimal> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a * b).ToList();
        }

        public static IEnumerable<double> Product(this IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a * b).ToList();
        }

        public static IEnumerable<decimal> Add(this IEnumerable<decimal> X, IEnumerable<decimal> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a + b).ToList();
        }

        public static IEnumerable<double> Add(this IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a + b).ToList();
        }

        public static IEnumerable<decimal> Sub(this IEnumerable<decimal> X, IEnumerable<decimal> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a - b).ToList();
        }

        public static IEnumerable<double> Sub(this IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a - b).ToList();
        }

        public static IEnumerable<decimal> Div(this IEnumerable<decimal> X, IEnumerable<decimal> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a / b).ToList();
        }

        public static IEnumerable<double> Div(this IEnumerable<double> X, IEnumerable<double> Y)
        {
            if (Y == null || X.Count() != Y.Count())
                throw new ArgumentException("Vector Y: size is not the same");
            return X.Zip(Y, (a, b) => a / b).ToList();
        }

        public static IEnumerable<T> Map<T>(this IEnumerable<T> X, Func<T, T> fn)
        {
            return X.Select(x => fn(x)).ToList();
        }
        #endregion

        #region Min/Max
        public static decimal MaxElem(this IEnumerable<decimal> X)
        {
            return X.Aggregate(default(decimal), (acc, n) => n > acc ? n : acc);
        }

        public static double MaxElem(this IEnumerable<double> X)
        {
            return X.Aggregate(default(double), (acc, n) => n > acc ? n : acc);
        }

        public static T MaxElem<T>(this IEnumerable<T> X) where T : IComparable<T>
        {
            return X.Aggregate(default(T), (acc, n) => n.CompareTo(acc) < 0 ? n : acc);
        }



        /// <summary>
        /// Find Index of the first max element of the Vector X
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>First index of the Max element of the Vector X</returns>
        public static int MaxIndex(this IEnumerable<decimal> X)
        {
            int i = 0, j = 0;
            decimal m = default(decimal);
            foreach(var e in X)
            {
                if(e > m)
                {
                    m = e;
                    j = i;
                }
                i++;
            }
            return j;
        }

        /// <summary>
        /// Find Index of the first max element of the Vector X
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>First index of the Max element of the Vector X</returns>
        public static int MaxIndex(this IEnumerable<double> X)
        {
            int i = 0, j = 0;
            double m = default(double);
            foreach (var e in X)
            {
                if (e > m)
                {
                    m = e;
                    j = i;
                }
                i++;
            }
            return j;
        }

        /// <summary>
        /// Find Fisrt Maximum with its index of the Vector X
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>Tuple (max, index)</returns>
        public static (decimal max, int index) MaxWithIndex(this IEnumerable<decimal> X)
        {
            return X.Zip(X.Enumerate(), (a, b) => (a, b)).Aggregate((default(decimal), default(int)), (acc, i) => i.a >  acc.Item1 ? i : acc);
        }
        /// <summary>
        ///  Find Fisrt Maximum with its index of the Vector X
        /// </summary>
        /// <param name="X">Vector X</param>
        /// <returns>Tuple (max, index)</returns>
        public static (double max, int index) MaxWithIndex(this IEnumerable<double> X)
        {
            return X.Zip(X.Enumerate(), (a, b) => (a, b)).Aggregate((default(double), default(int)), (acc, i) => i.a > acc.Item1 ? i : acc);
        }
        #endregion

        public static IEnumerable<int> Enumerate<T>(this IEnumerable<T> X)
        {
            int acc = 0;
            return X.Select(x => acc++).ToList();
        }

        public static String ToString<T>(this IEnumerable<T> X)
        {
            return "[" + X.Aggregate(string.Empty, (a, b) => string.IsNullOrEmpty(a) ? b?.ToString() ?? "null" : a.ToString() + " " + (b?.ToString() ?? "null")).Replace(',', '.').Replace(" ", ", ") + "]";
        }


        #region Matrix2D

        /// <summary>
        /// Multiplies Vector-Row X on Matrix2D M (Vector-Row * Matrix2D-Column)
        /// </summary>
        /// <param name="X">Vector-Row X</param>
        /// <param name="M">Matrix2D M</param>
        /// <returns>New Vector</returns>
        public static IEnumerable<double> Mull(this IEnumerable<double> X, Matrix2D M)
        {
            double[][] mData = M.Data;
            double[] result = new double[M.Cols];
            for(int i = 0; i < M.Cols; i++)
            {
                for (int j = 0; j < M.Rows; j++)
                    result[i] += mData[j][i] * X.ElementAt(i);
            }
            return result;
        }

        public static Matrix2D OuterProduct(this IEnumerable<double> X, IEnumerable<double> Y) {
            int XC = X.Count(), YC = Y.Count();

            double[][] result = new double[YC][];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = new double[XC];
            }
            for (int i = 0; i < XC; i++)
                for (int j = 0; j < YC; j++)
                    result[j][i] = X.ElementAt(i) * Y.ElementAt(j);
            

            return new Matrix2D(result);
        }
        #endregion


        #region Empty
        /// <summary>
        /// Returns IEnumerable&lt;<typeparamref name="T"/>&gt; sequence of n elements with specified value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Z"></param>
        /// <param name="size">count of elements</param>
        /// <param name="val">element itself.</param>
        /// <returns></returns>
        public static IEnumerable<T> EmptyArray<T>(this IEnumerable<double> Z, int size, T val)
        {
            return ((IEnumerable<T>)Array.CreateInstance(typeof(T), size)).Select(x => val).ToList();
        }


        /// <summary>
        /// Returns IEnumerable&lt;<typeparamref name="T"/>&gt; sequence of n elements with default value of type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumerable<T> EmptyArray<T>(this IEnumerable<double> Z,  int size)
        {
            T[] array = new T[size];
            return array;
        }
        #endregion
    }
}
