using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using WpfExplorer.Models;

namespace WpfExplorer.ViewModels
{
    public class TabPanelViewModel : ViewModelBaseWithGuid, INotifyPropertyChanged
    {

        private TabItemTemplateSelector<FileEditorModel> _headerTemplateSel;
        private TabItemTemplateSelector<FileEditorModel> _contentTemplateSel;

        public ObservableCollection<FileEditorModel> Files { get; } = new ObservableCollection<FileEditorModel>()
        {
            new FileEditorModel(), new FileEditorModel("+")
        };
        public TabPanelViewModel() {
            _headerTemplateSel = new TabItemTemplateSelector<FileEditorModel>();
            _contentTemplateSel = new TabItemTemplateSelector<FileEditorModel>();

            _headerTemplateSel.FilterKeys.Add((x)  => x.FileName.Equals("+") ? "addHeader" : "tabHeader");
            _contentTemplateSel.FilterKeys.Add((x) => x.FileName.Equals("+") ? "" : "tabItem");

            PropertyChanged += SelectedChange;
        }

        public void AddToSelectors(string key, DataTemplate dt)
        {
            if (_state == ViewModelState.Bound)
                return;

           _headerTemplateSel.DataTemplates.Add(key, dt);
           _contentTemplateSel.DataTemplates.Add(key, dt);
        }

        public DataTemplateSelector HeaderContentSelector { get { return _headerTemplateSel; } }
        public DataTemplateSelector ContentTemplateSelector { get { return _contentTemplateSel; } }

        public FileEditorModel SelectedItem { get; set; }

        private void SelectedChange(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == null || !e.PropertyName.Equals("SelectedItem"))
                return;
            var item = SelectedItem;
            if (item == null) return;

            if (item.FileName != null && item.FileName.Equals("+"))
            {
                string name = "new " + Files.Count;
                var ntabItem = new FileEditorModel(name);
                Files.Insert(Files.Count - 1, ntabItem);
                SelectedItem = ntabItem;
            }
        }


        //Close tab button command.
        private RelayCommand _closeTab;

        public RelayCommand CloseTabCommand
        {
            get
            {
                return _closeTab ??
                (
                  _closeTab = new RelayCommand(obj =>
                  {
                      int idx = -1;
                      if (obj != null && obj is FileEditorModel && Files.Count > 2) {
                          
                          BindingErrors.Hide(); //fix to TabStripPlacement Property binding.

                          FileEditorModel delTab = (FileEditorModel)obj;
                          idx = Files.IndexOf(delTab);
                          FileEditorModel prevTab = null;
                          
                          if (idx + 2 < Files.Count)
                               prevTab = Files[idx + 1]; //switch to next if middle tab deleted.
                          
                          else if(idx - 1 >= 0)
                              prevTab = Files[idx - 1]; //switch to prev tab if last tab deleted.


                          SelectedItem = prevTab;
                          
                          Files.Remove(delTab);
                          BindingErrors.Show();
                      }
                  })
                );
            }
        }
    }
}