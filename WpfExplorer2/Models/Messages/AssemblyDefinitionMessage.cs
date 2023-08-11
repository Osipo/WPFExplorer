using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models.Messages
{
    public class AssemblyDefinitionMessage : TextMessage, IMessage
    {
        private AssemblyDefinition _def;
        public AssemblyDefinitionMessage(AssemblyDefinition def, string msg) : base(msg) { 
            _def = def;
        }

        public AssemblyDefinition AssemblyDefinition { get { return _def; } }
    }
}
