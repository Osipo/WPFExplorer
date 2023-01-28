using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WpfExplorerControl.Extensions
{
    public static class NumericExtensions
    {

        #region precision of two values 
        /// <summary>
        /// Returns true if the values are close (difference smaller than eps)
        /// </summary>
        public static bool IsClose(this double d1, double d2, double eps)
        {
            if (d1 == d2) //Infinity
                return true;
            return Math.Abs(d1 - d2) < eps;
        }

        /// <summary>
        /// Returns true if difference between d1 and d2 values smaller than 0.01
        /// </summary>
        public static bool IsClose(this double d1, double d2)
        {
            return IsClose(d1, d2, 0.01);
        }

        /// <summary>
        /// Returns true if values are close (difference < eps)
        /// </summary>
        public static bool IsClose(this Size d1, Size d2, double eps)
        {
            return IsClose(d1.Width, d2.Width, eps) && IsClose(d1.Height, d2.Height, eps);
        }

        /// <summary>
        /// Returns true if difference between d1 and d2 values smaller than 0.01
        /// </summary>
        public static bool IsClose(this Size d1, Size d2)
        {
            return IsClose(d1.Width, d2.Width, 0.01) && IsClose(d1.Height, d2.Height, 0.01);
        }

        /// <summary>
        /// Returns true if values are close (difference < eps)
        /// </summary>
        public static bool IsClose(this Vector d1, Vector d2, double eps)
        {
            return IsClose(d1.X, d2.X, eps) && IsClose(d1.Y, d2.Y, eps);
        }

        /// <summary>
        /// Returns true if difference between d1 and d2 values smaller than 0.01
        /// </summary>
        public static bool IsClose(this Vector d1, Vector d2)
        {
            return IsClose(d1.X, d2.X, 0.01) && IsClose(d1.Y, d2.Y, 0.01);
        }
        #endregion

        #region DPI independence
        public static Rect TransformToDevice(this Rect rect, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return Rect.Transform(rect, matrix);
        }

        public static Rect TransformFromDevice(this Rect rect, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return Rect.Transform(rect, matrix);
        }

        public static Size TransformToDevice(this Size size, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
        }

        public static Size TransformFromDevice(this Size size, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new Size(size.Width * matrix.M11, size.Height * matrix.M22);
        }

        public static Point TransformToDevice(this Point point, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            return new Point(point.X * matrix.M11, point.Y * matrix.M22);
        }

        public static Point TransformFromDevice(this Point point, Visual visual)
        {
            Matrix matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformFromDevice;
            return new Point(point.X * matrix.M11, point.Y * matrix.M22);
        }
        #endregion
    }
}