using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfExplorer.Models
{
    public class SqlAssemblyObject
    {
        public string AssemblyName { get; set; }

        public SqlBytes Data { get; set; }
    }
}
