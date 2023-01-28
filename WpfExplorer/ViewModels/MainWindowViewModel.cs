using DevExpress.Mvvm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using WpfExplorer.Models;
using WpfExplorer.Screens;
using WpfExplorer.Services;

namespace WpfExplorer.ViewModels
{
    public class MainWindowViewModel : PageableViewModel
    {
        
        public Page CurrentPage { get; set; }

        private readonly MessageService _messageService;

        public MainWindowViewModel(PageService pageService, MessageService messageService) : base(pageService)
        {
            _messageService = messageService;
            _pageService.OnPageChanged += (page) => {
                if(CurrentPage != null && page != null && (page == CurrentPage || page.GetType() == CurrentPage.GetType())) return;
                CurrentPage = page;
            };
            var sub = _messageService.Subscribe<TextMessage>(StaticObjects.Reciever, async msg => {
                await Task.Delay(1);
                Console.WriteLine(msg);
            });
            
        }
       
        //block button element with this command until it is finished.
        //Creates new Page of type t.
        public System.Windows.Input.ICommand ChangePageCommand => new AsyncCommand<Type>(async (t) => {
            if (!typeof(Page).IsAssignableFrom(t))
                return;

            Page p = Activator.CreateInstance(t, args:null) as Page;
            _pageService.ChangePage(p);
            await _messageService.SendToAll(
                new TextMessage("(" + _messageService.Subscribers + ")" + " navigated to " + p.GetType().FullName)
            );
        });
    }
}
