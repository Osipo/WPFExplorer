using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfExplorer.Models;
using WpfExplorer.ViewModels;

namespace WpfExplorer.Screens
{
    /// <summary>
    /// Логика взаимодействия для TabsFileEditor.xaml
    /// </summary>
    public partial class TabsFileEditor : Page
    {
        public TabsFileEditor()
        {
            InitializeComponent();
            var vm = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).TabPanelViewModel;
            
            //Add templates to all template selectors of ViewModel
            foreach(DictionaryEntry entry in this.Resources)
            {
                if (entry.Value != null && entry.Value.GetType() == typeof(DataTemplate))
                {
                    vm.AddToSelectors((string)entry.Key, (DataTemplate)entry.Value);
                    
                }
            }

            //And atttach View-Model to panels.
            vm.SwitchContextOn(tabPanel);
        }
    }
}