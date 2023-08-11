using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Lists
{
    public class ProcItem : INotifyPropertyChanged
    {
        public string Name { get; set; }

        public string Content { get; set; }

        private ObservableCollection<ProcItem> _children;
        public ObservableCollection<ProcItem> Children { get { return _children; } }

        public object Value { get; set; }

        public AssemblyDefinition AssemblyDefinition { get; set; }

        public ProcItem(string name, string content) : this(name, content, false)
        {

        }

        public ProcItem(string name, string content, bool isSelected) {
            Name = name;
            Content = content;
            IsSelected = isSelected;
            _children = new ObservableCollection<ProcItem>();
        }

        public ProcItem(string name) : this(name, null, false) { }

        public ProcItem(string name, bool isSelected) : this(name, null, isSelected) { }

        private bool _isSelected = false;
        public bool IsSelected { 
            get { return _isSelected; }
            set
            {
                if(value != _isSelected) { _isSelected = value; OnPropertyChanged("IsSelected"); }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
