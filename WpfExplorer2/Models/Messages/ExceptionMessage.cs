using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public class ExceptionMessage : TextMessage, IMessage
    {

        private readonly Exception _ex;
        public ExceptionMessage(string msg, Exception e) : base(msg)
        {
            _ex = e;
        }

        public override string Message
        {
            get { return _ex.Message; }
        }

        public override string ToString()
        {
            return base.ToString() + "\n" + _ex.Message + "\n";
        }
    }
}
