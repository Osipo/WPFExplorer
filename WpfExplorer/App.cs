using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace WpfExplorer
{
    public class App : Application
    {

        public App()
        {
            this.Startup += new StartupEventHandler(App_Startup);
            this.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            this.Exit += new ExitEventHandler(App_Exit);
            this.SessionEnding += new SessionEndingCancelEventHandler(App_SessionEnding);
        }
        /// <summary>
        /// Launched by call app.Run();
        /// </summary>
        private void App_Startup(object sender, StartupEventArgs e)
        {
            ResourceDictionary appResources = new ResourceDictionary();
            appResources.Add("servicelocator", new ViewModelServiceLocator()); //add global servicelocator as Application Resource.
            this.Resources.MergedDictionaries.Add(appResources);

            ViewModelServiceLocator.Init();

            this.StartupUri = new System.Uri("Screens/MainWindow.xaml", System.UriKind.Relative);
        }

        /// <summary>
        /// Swallow exceptions (pass by).
        /// And log exceptions
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            e.Handled = true;
            MessageBox.Show(e.Exception.Message);
            if (App.Current != null)
                App.Current.Shutdown(-1);
        }

        private void App_Exit(object sender, ExitEventArgs e)
        {
            Console.WriteLine("Application exit with code " + e.ApplicationExitCode);
        }

        private void App_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            
        }
    }
}
