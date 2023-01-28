using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Workers
{
    public class CancelableBackgroundWorker : BackgroundWorker
    {
        public CancelableBackgroundWorker()
        {
            this.WorkerSupportsCancellation = true;
        }
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            try
            {
                base.OnDoWork(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Stop();
            }
        }
        public void Run()
        {
            if (this.IsBusy)
                return;
            this.RunWorkerAsync();
        }
        public void Stop()
        {
            this.CancelAsync();
            this.Dispose(true);
        }
    }
}
