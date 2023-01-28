using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public class StaticObjects
    {
        public static readonly object True = true;
        public static readonly object False = false;
        public static readonly object Reciever = new ViewModels.ViewModelBaseWithGuid();
    }
}
