using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.ViewModels
{
    public abstract class SelectionViewModelBaseWithGuid<T> : ViewModelBaseWithGuid
    {
        public T SelectedItem { get; set; }

        protected void SelectedChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || !e.PropertyName.Equals("SelectedItem"))
                return;

            var item = SelectedItem;
            if (item == null) return;
            ProcessSelected(item);
        }

        protected abstract void ProcessSelected(T item);
    }
}
