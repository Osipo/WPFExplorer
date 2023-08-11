using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Services;

namespace WpfExplorer.ViewModels
{
    public class TilesViewModel : PageableViewModel
    {
        protected readonly MessageService _messageService;
        public TilesViewModel(PageService pageService, MessageService messageService) : base(pageService) {
            _messageService = messageService;
        }

        public int LineWidth { get; set; }

    }
}
