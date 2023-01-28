using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
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
using WpfExplorer.Workers;

namespace WpfExplorer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            var vm = (App.Current.Resources["servicelocator"] as ViewModelServiceLocator).MainViewModel;
            vm.SwitchContextOn(this);

            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("ru-ru"); //en-US
            InitializeComponent();

            TimerBackgroundWorker w = new TimerBackgroundWorker(); //default interval = 1 second. cyclic worker.
            w.DoWork += (sender, e) => { Dispatcher.Invoke(UpdateClockDisplay); };
            w.Start();
        }

        private void ExceptionBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("Generated exception by button click.");
            
        }


        private void ExitBtn_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(1);
        }
        
        private void UpdateClockDisplay()
        {
            CultureInfo ci = CultureInfo.CurrentCulture;
            DateTime dt = DateTime.Now;
            textDate.Text = dt.ToString("F"); //F - full LocalDateTime, U - full UniversalDateTime (UTC)
        }
    }
}
