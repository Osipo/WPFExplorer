using DevExpress.Mvvm;
using ICSharpCode.AvalonEdit.Document;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfExplorer.ViewModels;

namespace WpfExplorer.Models
{
    public class FileEditorModel : ViewModelBaseWithGuid, INotifyPropertyChanged
    {

        private string _filePath;
        private string _fileName;
        private string _newFileName;
        private string _filecontent;
        private double _progress;
        private TextDocument _doc;

        public FileEditorModel() : this("new 1")
        {
        }

        public FileEditorModel(string fileName)
        {
            _filePath = null;
            _fileName = fileName;
            _newFileName = fileName;
            _filecontent = "";
            _progress = 0.0;
            flushDocument();
        }

        private void flushDocument()
        {
            Document = new TextDocument();
            Document.TextChanged += (s, e) => { FileContent = Document.Text; };
            Document.Text = FileContent;
        }

        public String FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        public String FilePath
        {
            get { return _filePath; }
            set { _filePath = value;  }
        }
        public String FileContent
        {
            get { return _filecontent; }
            set { _filecontent = value; }
        }

        public TextDocument  Document { get { return _doc; } set { _doc = value; } }

        public Double Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        public System.Windows.Input.ICommand CloseFileCommand => new DelegateCommand(() =>
        {
            Progress = 0.0;
            FileContent = "";
            FilePath = null;
            FileName = _newFileName;
            flushDocument();
            
        });

        public System.Windows.Input.ICommand OpenFileCommand => new DelegateCommand(() => {
            OpenFileDialog fdialog = new Microsoft.Win32.OpenFileDialog();
            if (fdialog.ShowDialog() == true)
            {
                FilePath = fdialog.FileName;
                FileName = Path.GetFileName(FilePath);

                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += new DoWorkEventHandler(DoReadFileAsync);
                worker.RunWorkerAsync(FilePath);
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
}
