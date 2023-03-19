using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.ViewModels;

namespace WpfExplorer.Models
{
    public class ConnectionItem : ViewModelBaseWithGuid, INotifyPropertyChanged
    {
        private string _connectionName;
        private string _connectionString;

        public ConnectionItem(string name) { 
            _connectionName = name;
        }

        public String ConnectionName { get { return _connectionName; } }

        public override string ToString()
        {
            return _connectionName;
        }
    }
}
