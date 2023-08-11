using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models.Lists;
using WpfExplorer.Models;
using WpfExplorer.Models.Messages;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using Mono.Cecil;
using ICSharpCode.Decompiler.TypeSystem;

namespace WpfExplorer.ViewModels
{
    public class ProceduresTabItemTreeViewModel : TabItemTreeViewModel<FileEditorModel, ProcItem>
    {

        public ObservableCollection<ProcItem> Procedures { get; } = new ObservableCollection<ProcItem>();

        public ProceduresTabItemTreeViewModel(FileEditorModel parent) : base(parent)
        {
            var ms = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).MessageService; //global Service
            ms.Subscribe<StmtMessage>(this, async msg => {
                await App.Current.Dispatcher.InvokeAsync(() => {

                    //create new ProcItem and add it into collection.
                    ProcItem nitem = new ProcItem(msg.Header, msg.Message, true);
                    Procedures.Add(nitem);
                    SelectedItem = nitem;
                });
            });


            ms.Subscribe<AssemblyDefinitionMessage>(this, async msg =>
            {
                await App.Current.Dispatcher.InvokeAsync(() => {

                    ProcItem root = new ProcItem(msg.Message, string.Empty, true);
                    
                    foreach (var typeInAssembly in msg.AssemblyDefinition.MainModule.Types)
                    {
                        if (typeInAssembly.IsPublic)
                        {
                            ProcItem ct = new ProcItem(typeInAssembly.FullName, true);
                            ct.AssemblyDefinition = msg.AssemblyDefinition;
                            foreach (var item in typeInAssembly.Methods)
                            {
                                ProcItem node = new ProcItem(item.Name, true);
                                node.Value = item;
                                node.AssemblyDefinition = msg.AssemblyDefinition;
                                ct.Children.Add(node);
                            }

                            foreach (var item in typeInAssembly.Properties)
                            {
                                ProcItem node = new ProcItem(item.Name, true);
                                node.Value = item;
                                node.AssemblyDefinition = msg.AssemblyDefinition;
                                ct.Children.Add(node);
                            }

                            foreach (var item in typeInAssembly.Fields)
                            {
                                ProcItem node = new ProcItem(item.Name, true);
                                node.Value = item;
                                node.AssemblyDefinition = msg.AssemblyDefinition;
                                ct.Children.Add(node);
                            }
                            root.Children.Add(ct);
                        }
                    }

                    Procedures.Add(root);
                    SelectedItem = root;
                });
            });
        }

        public override void SwitchContextOn(FrameworkElement v)
        {
            base.SwitchContextOn(v);
            if(v is TreeView)
            {
                (v as TreeView).SelectedItemChanged += (o, e) => {
                    if (e.NewValue is ProcItem)
                        SelectedItem = e.NewValue as ProcItem;
                };
            }
        }


        protected override void ProcessSelectedWithParent(FileEditorModel pItem, ProcItem item)
        {
            if (item.Value is IMemberDefinition && item.AssemblyDefinition != null)
            {
                var decompiler = new CSharpDecompiler(item.AssemblyDefinition.MainModule, new DecompilerSettings());
                pItem.FileContent = decompiler.DecompileAsString(item.Value as IMemberDefinition);
                
            }
            else
                pItem.FileContent = item.Content;
        }

        private RelayCommand _clearTree;

        public RelayCommand ClearCommand {
            get
            {
                return _clearTree ??
                (
                    _clearTree = new RelayCommand(obj => {
                        Procedures.Clear();
                        _parent.FileContent = "";
                        SelectedItem = null;
                    })
                );
            }
        }

        //Just for tests.
        private RelayCommand _addTest;
        public RelayCommand AddTestCommand
        {
            get
            {
                return _addTest ??
                (
                    _addTest = new RelayCommand(obj => {
                        Procedures.Add(new ProcItem("testItem" + Procedures.Count, "test" + +Procedures.Count, true));
                        SelectedItem = Procedures[Procedures.Count - 1];
                    })
                );
            }
        }
    }
}
