using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfExplorer.Workers
{
    public  class TimerBackgroundWorker : BackgroundWorker
    {
        private ManualResetEvent intervalManualReset;
        private enum ProcessStatus { Created, Running, JobCompleted, ExceptionOccured };
        private ProcessStatus processStatus = new ProcessStatus();
        public TimeSpan Interval { get; set; }

        public TimerBackgroundWorker(TimeSpan t)
        {
            this.processStatus = ProcessStatus.Created;
            this.WorkerSupportsCancellation = true;
            this.Interval = t;
        }

        public TimerBackgroundWorker() : this(TimeSpan.FromSeconds(1)) {
            
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            base.OnRunWorkerCompleted(e);
            if (processStatus == ProcessStatus.ExceptionOccured)
            {

            } 
            processStatus = ProcessStatus.JobCompleted;
        }
        protected override void OnDoWork(DoWorkEventArgs e)
        {
            while (!this.CancellationPending)
            {
                try
                {
                    base.OnDoWork(e);
                    this.Sleep();
                }
                catch (OperationCanceledException)
                {

                }
                catch (Exception ex)
                {
                    // Log ...
                    Console.WriteLine(ex.ToString());
                    this.processStatus = ProcessStatus.ExceptionOccured;
                    this.Stop();
                }
            }
            if (e != null)
                e.Cancel = true;
        }

        public void Start()
        {
            this.processStatus = ProcessStatus.Running;
            if (this.IsBusy)
                return;

            this.intervalManualReset = new ManualResetEvent(false);
            this.RunWorkerAsync();
        }
        public void Stop()
        {
            this.CancelAsync();
            this.intervalManualReset?.Set(); //unblock all threads.
            this.Dispose(true);
        }

        private void Sleep()
        {
            if (this.intervalManualReset != null)
            {
                this.intervalManualReset.Reset(); //call reset (manually) to block all threads.
                this.intervalManualReset.WaitOne(this.Interval); //wait 1 second for signal.
            }
        }
    }
}
