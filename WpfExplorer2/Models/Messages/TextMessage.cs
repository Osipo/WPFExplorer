using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public class TextMessage : IMessage
    {
        protected string _message;
        public virtual string Message { get { return _message; } }

        public TextMessage(string msg) {
            _message= msg;
        }

        public override string ToString()
        {
            return _message;
        }
    }
}
