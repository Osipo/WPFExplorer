using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using WpfExplorer.Behaviours;
using WpfExplorer.Models.Lists;
using WpfExplorer.Models.Messages;
using WpfExplorer.Models.ML;
using WpfExplorer.Models.Text;
using WpfExplorer.Screens;
using WpfExplorer.ViewModels;
using WpfExplorerControl.Extensions;
using static System.Net.WebRequestMethods;

namespace WpfExplorer.Models
{

    /* Content of the Tab. DataContext is the DataTemplate. */
    public class FileEditorModel : IGuid, INotifyPropertyChanged //ViewModelBaseWithGuid, INotifyPropertyChanged
    {

        private string _filePath;
        private string _fileName;
        private string _newFileName;
        private string _filecontent;
        private double _progress;


        private TextDocument _doc;
        private WordsInfo _words;
        private TextPosition _textPosition;
        private TextFindReplace _textFindReplace;
        private ProceduresTabItemTreeViewModel _subTreeVM;
        private TextEditor _editor; //get CarretPosition from editor.
        

        private SQLDbWorker _dbWorker;
        private DataGrid _grid;
        private SearchTree _searchtree;



        private bool _selected = false;


        private Guid _id;
        public event PropertyChangedEventHandler PropertyChanged;

        public FileEditorModel() : this("new 1")
        {
        }

        public void SetEditor(TextEditor editor)
        {
            _editor = editor;
        }
       
        public FileEditorModel(string fileName)
        {
            _id = Guid.NewGuid();
            _filePath = null;
            _fileName = fileName;
            _newFileName = fileName;
            _filecontent = "";
            _progress = 0.0;
            _words = new WordsInfo();
            _textPosition = new TextPosition();
            _textFindReplace = new TextFindReplace();
            _subTreeVM = new ProceduresTabItemTreeViewModel(this);
            _searchtree = new SearchTree();

            //Registrate one message for this Tab.
            //Each tab is also listener (ls) for Message TextMessage.
            var ms = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).MessageService;
            if (ms != null)
            {
                ms.Subscribe<TextMessage>(this, async msg =>
                {
                    await Task.Delay(1);
                    Console.WriteLine(msg);
                });
            }

            PropertyChanged += new PropertyChangedEventHandler(PropertyChangedHandler);
            flushDocument();
        }

        public int CarretOffset
        {
            get; set;
        }

        public void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "FileContent")
            {
                _words.NotifyAboutContent(FileContent);
            }
            else if (e.PropertyName == "CarretOffset")
            {
                TextLocation c = Document.GetLocation(CarretOffset);
                _textPosition.Row = c.Line;
                _textPosition.Column = c.Column;
                _textPosition.PositionStr = c.Line + ":" + c.Column;
                _textPosition.RaisePropertyChanged("PositionStr");
            }
        }

        public void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void flushDocument()
        {
            Document = new TextDocument();

            Document.TextChanged += (s, e) => {
                int off = _editor.CaretOffset;
                FileContent = Document.Text;
                _editor.CaretOffset = off; //Preserve carret offset that points to the last inserted/deleted symbol from keyboard.
                CarretOffset = off;
                if(_subTreeVM != null && _subTreeVM.SelectedItem != null)
                {
                    _subTreeVM.SelectedItem.Content = FileContent;
                }

            };
            Document.Text = FileContent;
        }

        private void updateGrid(DataTable dt)
        {
            Grid.ItemsSource = null;
            Grid.Columns.Clear();
            Grid.AutoGenerateColumns = true;
            Grid.ItemsSource = dt.DefaultView;
            Grid.UpdateLayout();
            Grid.Items.Refresh();
        }

        public Guid Guid {  get { return _id; } }

        public bool Selected
        {
            get { return _selected; }
            set { _selected = value; }
        }

        public String FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public String FilePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }
        public String FileContent
        {
            get { return _filecontent; }
            set { _filecontent = value; }
        }

        public TextDocument Document { get { return _doc; } set { _doc = value; } }

        public Double Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public DataGrid Grid { get { return _grid; } set { _grid = value; } }

        public SearchTree SearchTree => _searchtree;
        public WordsInfo Words { get { return _words; } }

        public TextPosition TextPosition { get { return _textPosition; } }

        public TextFindReplace FindReplace { get { return _textFindReplace; } }

        public ProceduresTabItemTreeViewModel ProcedureTreeViewModel { get { return _subTreeVM; } }

        public System.Windows.Input.ICommand CloseFileCommand => new DelegateCommand(() =>
        {
            Progress = 0.0;
            FileContent = "";
            FilePath = null;
            FileName = _newFileName;
            flushDocument();
        });

        public void Dispose()
        {
            _dbWorker?.Dispose(); //free connection.
        }

        public System.Windows.Input.ICommand OpenFileCommand => new DelegateCommand(() => {
            OpenFileDialog fdialog = new Microsoft.Win32.OpenFileDialog();
            if (fdialog.ShowDialog() == true)
            {
                FilePath = fdialog.FileName;
                FileName = Path.GetFileName(FilePath);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(DoReadFileAsync);
                worker.RunWorkerAsync(FilePath); //do not block UI.
            }
        });

        public System.Windows.Input.ICommand SaveFileCommand => new AsyncCommand(async () =>
        {
            if (FilePath == null)
            {
                SaveFileDialog fdialog = new SaveFileDialog();
                if (fdialog.ShowDialog() == true)
                    FilePath = fdialog.FileName;
            }
            if (FilePath == null)
                return;
            FileName = Path.GetFileName(FilePath);
            await DoSaveFileAsync(FilePath); //wait UI til whole content saved.
        });

        public System.Windows.Input.ICommand ConnectCommand => new DelegateCommand(() =>
        {
           ConnectionFormWindow w = new ConnectionFormWindow(true);
           w.Owner = App.Current.MainWindow;
           if(w.ShowDialog() == true)
           {
                if(_dbWorker != null)
                {
                    _dbWorker.Dispose();
                }
                _dbWorker = new SQLDbWorker(w.ConnectionString);
                _searchtree.SQLDbWorker = _dbWorker; 
           }
        });

        public System.Windows.Input.ICommand ExecuteCommand => new DelegateCommand(async () =>
        {

            //Get Message Service for sending messages.
            var ms = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).MessageService;

            //IDENTIFY TYPE OF COMMAND.
            //TODO: Make Enumerable Set of Commands
            var sql = new string(FileContent.ToCharArray());
            sql = sql.TrimStart(StaticObjects.EcmaSeparators);
            sql = sql.ToLower();

            //SHOW FILE COMMAND
            if(sql.StartsWith("show file "))
            {
                string fileName = null;
                try
                {
                    fileName = sql.Split(StaticObjects.EcmaSeparators, StringSplitOptions.RemoveEmptyEntries)[2];

                    //Parse Sql Bytes of objects into Assembly Definition
                    var pars = new ReaderParameters();
                    pars.InMemory = true;
                   
                    var adef = Mono.Cecil.AssemblyDefinition.ReadAssembly(fileName, pars);
                    await ms.SendTo(new AssemblyDefinitionMessage(adef, adef.Name.Name), _subTreeVM);
                } 
                catch(IndexOutOfRangeException e)
                {
                    FileContent = "'show file {fileName}': Specify file name";
                }
                catch(Exception e)
                {
                    FileContent = "file " + fileName + " is not a CLR Assembly!";
                }
                return;
            }

            else if (_dbWorker == null)
                return;

            //SHOW COMMAND
            else if (sql.StartsWith("show")) //show command => get description of object
            {
                var procName = sql.Split(StaticObjects.EcmaSeparators, StringSplitOptions.RemoveEmptyEntries)[1]; //the name after 'show' command
                var result = await _dbWorker.GetObjectDefinitionAsync(procName);

                if (string.IsNullOrWhiteSpace(result)) //not procedure.
                {
                    //Try Get assembly objects.
                    List<SqlAssemblyObject> a = await _dbWorker.GetAssemblyDefinitionAsync(procName);
                    int l  = a.Count;
                    if(l == 0)
                    {
                        FileContent = "Cannot find object '" + procName + "'";
                        return;
                    }
                    
                    //Parse Sql Bytes of objects into Assembly Definition
                    var pars = new ReaderParameters();
                    pars.AssemblyResolver = new DatabaseAssemblyResolver(a);

                    for (int i = 0; i < l; i++)
                    {
                        var adef = Mono.Cecil.AssemblyDefinition.ReadAssembly(new MemoryStream(a[i].Data.Value), pars);
                        await ms.SendTo(new AssemblyDefinitionMessage(adef, procName), _subTreeVM);
                    }
                }

                //Send result as StmtMessage into ProcTabItemViewModel
                //By sending it into its listener.
                await ms.SendTo(new StmtMessage(result, procName), _subTreeVM);
                return;
            }

            //FIND COMMAND
            else if (sql.StartsWith("find"))
            {
                var searchStr = sql.Split(new[] { ' ', '\t', '\n', '\r', '\v', '\f' }, StringSplitOptions.RemoveEmptyEntries)[1]; //the str after 'find' command
                var result = await _dbWorker.FindString(searchStr);
                
                
                foreach (var proc in result)
                {
                    await ms.SendTo(new StmtMessage(proc.Value, proc.Key), _subTreeVM);
                }
                return;
            }

            //CREATE OR ALTER COMMAND
            else if (sql.StartsWith("ALTER") || sql.StartsWith("CREATE") || sql.StartsWith("CREATE OR ALTER"))
            {
                var b = await _dbWorker.AlterProcedureAsync(sql);
                return;
            }


            //Regular SELECT query. SELECT COMMAND
            DataSet ds = await _dbWorker.QueryAsync(FileContent);
            if (ds == null || ds.Tables.Count <= 0) return;

            DataTable dt = ds.Tables[0];

            Console.WriteLine("cols: " + dt.Columns.Count + ", rows: " + dt.Rows.Count);

            
            var grid = Grid;
            App.Current.Dispatcher.Invoke(new RefreshGrid(updateGrid), dt);
            
        });

        //TreeView Commands
        public System.Windows.Input.ICommand ApplyAllCommand => new AsyncCommand(async () =>
        {
            foreach(var proc in _subTreeVM.Procedures)
            {
                var b = await _dbWorker.AlterProcedureAsync(proc.Content);
            }
        });

        public System.Windows.Input.ICommand FindReplaceCommand
        {
            get
            {
                return new DelegateCommand(() => {

                    
                    string search = FindReplace.SearchString;
                    string replace = FindReplace.ReplaceString;
                    if (FindReplace.IsGlobal)
                    {
                        foreach (var p in ProcedureTreeViewModel.Procedures)
                        {
                            StringBuilder sb = new StringBuilder();
                            foreach (var s in p.Content.Split(StaticObjects.Space, StringSplitOptions.RemoveEmptyEntries))
                            {
                                sb.Append(s.Replace(search, replace)).Append(' ');
                            }
                            //Console.WriteLine("Content " + sb.ToString());
                            p.Content = sb.ToString();
                        }
                        FileContent = ProcedureTreeViewModel.SelectedItem.Content;
                    }
                    else
                    {
                        var p = ProcedureTreeViewModel.SelectedItem;
                        if (p == null)
                            return;
                        StringBuilder sb = new StringBuilder();
                        foreach (var s in FileContent.Split(StaticObjects.Space, StringSplitOptions.RemoveEmptyEntries))
                        {
                            sb.Append(s.Replace(search, replace)).Append(' ');
                        }
                        p.Content = sb.ToString();
                        FileContent = p.Content;
                    }
                });
            }
        }

        /*
        public System.Windows.Input.ICommand ContinueTextCommand => new DelegateCommand(async () => {

            
            await Task.Delay(5000); //wait 5 sec.

            FileContent = "Random text generated.";
            flushDocument();
            
            
            //call service to show all TextMessages for this tab.
            var ms = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).MessageService;
            if (ms != null)
                await ms.SendTo(new TextMessage(_fileName + ": " + " continueTextCommand finished."), this);
        });
        */

        private async void DoReadFileAsync(object sender, DoWorkEventArgs e)
        {
            string filePath = e.Argument.ToString();
            byte[] buf = new byte[4096];
            StringBuilder sb = new StringBuilder();
            int readBytes = 0;
            long curBytes = 0;
            using (FileStream reader = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true))
            {
                while ((readBytes = await reader.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    sb.Append(System.Text.Encoding.UTF8.GetString(buf, 0, readBytes));
                    curBytes += readBytes;
                    Progress = ((double)curBytes / reader.Length); //ratio of bytes for progressBar.
                }
            }
            FileContent = sb.ToString();
            App.Current.Dispatcher.Invoke(flushDocument);
            
            sb.Clear();
        }

        private async Task DoSaveFileAsync(string filePath)
        {
            //string filePath = e.Argument.ToString();
            int line = 0;
            string[] lines = FileContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            using (FileStream writer = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite, bufferSize: 4096 * sizeof(char), useAsync: true))
            {
                using (StreamWriter sw = new StreamWriter(writer, Encoding.UTF8))
                {
                    while (line < lines.Length)
                    {
                        await sw.WriteLineAsync(lines[line]);
                        line++;
                    }
                }
            }
        }
      
    }

    internal delegate void RefreshGrid(DataTable dt);
}
