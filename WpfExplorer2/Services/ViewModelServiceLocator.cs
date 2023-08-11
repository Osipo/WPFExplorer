using DevExpress.Mvvm.POCO;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Models;
using WpfExplorer.Services;
using WpfExplorer.ViewModels;

namespace WpfExplorer
{
    public class ViewModelServiceLocator
    {
        private static IServiceProvider _provider;
        
        public static void Init()
        {
            
            ServiceCollection services = new ServiceCollection();

            services.AddSingleton<MainWindowViewModel>();

            services.AddScoped<TilesViewModel>();
            services.AddScoped<TabPanelViewModel>();

            services.AddSingleton<PageService>();
            services.AddSingleton<MessageService>(); //has at least 4 services: mainVM, two tabs, and ExceptionService.
            services.AddSingleton<ExceptionService>(); 

            _provider = services.BuildServiceProvider();
            foreach(var service in services)
            {
                _provider.GetRequiredService(service.ServiceType);
            }
            
        }

        public MainWindowViewModel MainViewModel { get { return _provider.GetRequiredService<MainWindowViewModel>(); } }


        public TilesViewModel TilesViewModel { get { return _provider.GetRequiredService<TilesViewModel>(); } }

        public TabPanelViewModel TabPanelViewModel { get { return _provider.GetRequiredService<TabPanelViewModel>(); } }

        public MessageService MessageService { get { return _provider.GetRequiredService<MessageService>(); } }

        public ExceptionService ExceptionService { get { return _provider.GetRequiredService<ExceptionService>(); } }
    }
}
