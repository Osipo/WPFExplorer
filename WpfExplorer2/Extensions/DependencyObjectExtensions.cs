using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Runtime.CompilerServices;
using WpfExplorerControl.Control;

namespace WpfExplorerControl.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject rootObject, Predicate<T> condition) where T : DependencyObject
        {
            if (rootObject != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rootObject); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(rootObject, i);

                    if (child != null && child is T && condition((T)child) )
                        yield return (T)child;

                    foreach (T childOfChild in FindVisualChildren<T>(child, condition))
                        yield return childOfChild;
                }
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject rootObject) where T : DependencyObject
        {
            return rootObject.FindVisualChildren<T>(x => true); //with no condition
        }

        public static T FindVisualChild<T>(this DependencyObject parent, Predicate<T> condition) where T : DependencyObject
        {
            int chl = VisualTreeHelper.GetChildrenCount(parent);
            for (int childCount = 0;
                childCount < chl; childCount++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, childCount);
                if (child != null && child is T && condition.Invoke((T)child))
                    return (T)child;
                else
                {
                    T childOfChild = FindVisualChild<T>(child, condition);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static T FindVisualParent<T>(this DependencyObject child) where T : DependencyObject
        {
            var p = VisualTreeHelper.GetParent(child);
            while(p != null && !(p is T)) p = VisualTreeHelper.GetParent(p);
            if(p is T)
                return (T)p;
            return null;
        }

        public static T FindVisualChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            return parent.FindVisualChild<T>(x => true); //with no condition.
        }
    }
}
