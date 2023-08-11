using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Workers;

namespace WpfExplorer.Models.Lists
{
    public class SearchTree : INotifyPropertyChanged
    {
        
        public String Search { get; set; }

        public SQLDbWorker SQLDbWorker { get; set; }

        public System.Windows.Input.ICommand FindTreeCommand => new DelegateCommand(async () => {
            var s = Search;
            if (string.IsNullOrWhiteSpace(s)) return;
            if (SQLDbWorker == null) return;

            CancelableBackgroundWorker job = new CancelableBackgroundWorker();
            job.DoWork += new DoWorkEventHandler(DoSearchJob);
            job.Run();

        });

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void DoSearchJob(object sender, DoWorkEventArgs e)
        {
            
        }
    }
}