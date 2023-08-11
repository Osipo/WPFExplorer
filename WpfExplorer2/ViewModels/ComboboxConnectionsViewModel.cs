using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfExplorer.Models;

namespace WpfExplorer.ViewModels
{
    public class ComboboxConnectionsViewModel : SelectionViewModelBaseWithGuid<ConnectionItem>, INotifyPropertyChanged
    {
        public ObservableCollection<ConnectionItem> Connections { get; } = new ObservableCollection<ConnectionItem>()
        {
            new ConnectionItem("MS SQL"), new ConnectionItem("Postgresql"), new ConnectionItem("Add new connection.")
        };

        public ComboboxConnectionsViewModel() {
            PropertyChanged += SelectedChange;
        }

        protected override void ProcessSelected(ConnectionItem item)
        {
            Console.WriteLine($"Selected: {SelectedItem?.ConnectionName}");
        }

        public System.Windows.Input.ICommand ShowSelectedComboItem => new DelegateCommand(() =>
        {
            MessageBox.Show($"Selected: {SelectedItem?.ConnectionName}");
        });
    }
}
