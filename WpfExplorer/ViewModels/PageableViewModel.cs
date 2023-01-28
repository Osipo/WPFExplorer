using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfExplorer.Services;

namespace WpfExplorer.ViewModels
{
    public class PageableViewModel : ViewModelBaseWithGuid
    {
        protected readonly PageService _pageService;

        public PageableViewModel(PageService pageService)
        {
            _pageService = pageService;
        }
    }
}
