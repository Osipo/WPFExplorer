using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WpfExplorer.ViewModels
{
    public class ViewModelBaseWithGuid : ViewModelBase
    {
        private Guid _guid;
        protected ViewModelState _state;
        protected FrameworkElement _view;
        public ViewModelBaseWithGuid()
        {
            _guid = Guid.NewGuid();
            _state = ViewModelState.Initial;
        }
        public Guid Guid { get { return _guid;} }

        public FrameworkElement View { get { return _view; } }

        public ViewModelState State { get { return _state; } }

        public virtual void SwitchContextOn(FrameworkElement v) { _view = v; _view.DataContext = this; _state = ViewModelState.Bound; } 
    }
}
