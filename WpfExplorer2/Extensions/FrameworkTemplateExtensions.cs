using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExplorerControl.Extensions
{
    public static class FrameworkTemplateExtensions
    {
        /// <summary>
        /// Creates new Element from VisualTree of DataTemplate.
        /// </summary>
        /// <typeparam name="T">Type of Element</typeparam>
        /// <param name="obj"></param>
        /// <param name="condition">Predicate</param>
        /// <returns></returns>
        public static T GetVisualRoot<T>(this FrameworkTemplate obj, Predicate<T> condition) where T : DependencyObject
        {
            var parent = obj.LoadContent() as FrameworkElement;
            return parent.FindVisualChild<T>(condition);
        }
    }
}
