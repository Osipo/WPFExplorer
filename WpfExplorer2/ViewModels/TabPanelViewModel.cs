using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using WpfExplorer.Models;
using WpfExplorer.Models.Messages;
using WpfExplorer.Services;
using WpfExplorerControl.Control;
using WpfExplorerControl.Extensions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace WpfExplorer.ViewModels
{
    /* This class is View-Model of TabControl (_view is ref to TabControl element) */
    public class TabPanelViewModel : SelectionViewModelBaseWithGuid<FileEditorModel> /*ViewModelBaseWithGuid*/ , INotifyPropertyChanged
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

        public void OnProcessSelected(FileEditorModel item)
        {
            ProcessSelected(item);
        }

        protected override void ProcessSelected(FileEditorModel item)
        {
            if (item.FileName != null && item.FileName.Equals("+"))
            {
                string name = "new " + Files.Count;
                var ntabItem = new FileEditorModel(name); //Notify auto about new instance of Document.

                Files.Insert(Files.Count - 1, ntabItem);
                SelectedItem = ntabItem;
            }
            else
                item.RaisePropertyChanged("FileContent"); //Notify about FileContent when Document changed (from cache)
        }


        public static bool GetContentLoaded(DependencyObject obj)
        {
            return (bool)obj.GetValue(ContentLoadedProperty);
        }
        public static void SetContentLoaded(DependencyObject obj, bool value)
        {
            obj.SetValue(ContentLoadedProperty, value);
        }

        /// <summary>
        /// Controls whether tab content is loaded by DataTemplate.
        /// </summary>
        /// <remarks>When TabContent.ContentLoaded is true, DataTemplate.LoadContent() was executed and returned new UIElement that is root of the ContentTemplate of the tab.</remarks>
        public static readonly DependencyProperty ContentLoadedProperty =
            DependencyProperty.RegisterAttached("ContentLoaded", typeof(bool), typeof(TabPanelViewModel), new UIPropertyMetadata(false, OnContentLoadedChanged));

        //When Element obj is in DataTemplate has Property TabPanelViewModel.ContentLoaded=true then execute this method with new found Element.
        private static void OnContentLoadedChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {

            if (obj == null) return;
            bool newValue = (bool)args.NewValue;
            if (!newValue)
            {
                if (args.OldValue != null && ((bool)args.OldValue))
                {
                    throw new NotImplementedException("Cannot change TabPanelViewModel.ContentLoaded from True to False!");
                }
                return;
            }
            if (obj is TreeView)
            {
                var tv = obj as TreeView;
                var i1 = tv.FindVisualParent<TabControl>()?.SelectedItem as FileEditorModel;
                i1?.ProcedureTreeViewModel?.SwitchContextOn(tv);
            }
            else if(obj is TextEditor)
            {
                var te = obj as TextEditor;
                var i2 = te.FindVisualParent<TabControl>()?.SelectedItem as FileEditorModel;
                
                //Set editor and its syntax highligting of MS SQL 2008 by default.
                //Layed in /Resources dir as Embedded Resource
                i2?.SetEditor(te);
                var a = Assembly.GetAssembly(typeof(TabPanelViewModel));
                using (Stream s = a.GetManifestResourceStream(a.GetName().Name + ".Resources.SqlSyntax.xshd")) //binary stream
                {
                    using (XmlTextReader reader = new XmlTextReader(s))
                    {
                        te.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
            else if (obj is DataGrid)
            {

                var g = obj as DataGrid;
                var t = g.FindVisualParent<TabControl>();
                if (t == null || !(t.SelectedItem is FileEditorModel)) return;
                var i = t.SelectedItem as FileEditorModel;
                i.Grid = g; //We need Grid into model. (Corrupted MVVM)
            }
        }



        public DataTemplateSelector HeaderContentSelector { get { return _headerTemplateSel; } }
        public DataTemplateSelector ContentTemplateSelector { get { return _contentTemplateSel; } }


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
                          
                          delTab.Dispose();
                          Files.Remove(delTab);
                          BindingErrors.Show();
                      }
                  })
                );
            }
        }

    }
}