using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Messages
{
    public class StmtMessage : TextMessage, IMessage
    {
        public StmtMessage(string msg, string header) : base(msg)
        {
            _header = header;
        }

        private string _header;
        public string Header { get { return _header; } }
    }
}
