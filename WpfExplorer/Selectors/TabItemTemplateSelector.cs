using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using WpfExplorer.ViewModels;

namespace WpfExplorer
{
    public class TabItemTemplateSelector<T> : DataTemplateSelector where T : class
    {
        private readonly Dictionary<string, DataTemplate> dataTemplates =
                 new Dictionary<string, DataTemplate>();

        private readonly HashSet<Func<T, string>> _filters = new HashSet<Func<T, string>>();

        public HashSet<Func<T, string>> FilterKeys { get { return _filters; } }

        public Dictionary<string, DataTemplate> DataTemplates { get { return dataTemplates; } }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate result = null;
            if (item.GetType() != typeof(T))
                return null;
            
            foreach(Func<T, String> f in _filters)
            {
                dataTemplates.TryGetValue(f((T)item), out result);
                if (result != null)
                    return result;
            }

            return result;
        }
    }
}
