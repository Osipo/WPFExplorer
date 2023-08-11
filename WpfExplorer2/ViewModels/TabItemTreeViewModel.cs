using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfExplorer.Models.Lists;

namespace WpfExplorer.ViewModels
{
    public abstract class TabItemTreeViewModel<P, C> : SelectionViewModelBaseWithGuid<C>, INotifyPropertyChanged
    {

        protected readonly P _parent;
        public TabItemTreeViewModel(P parent)
        {
            _parent = parent;
            PropertyChanged += SelectedChange;
        }

        protected override void ProcessSelected(C item)
        {
            if(_parent != null && item != null)
                ProcessSelectedWithParent(_parent, item);
        }

        protected abstract void ProcessSelectedWithParent(P pItem, C item);
    }
}
