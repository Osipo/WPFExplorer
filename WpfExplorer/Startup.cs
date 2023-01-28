using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WpfExplorer
{
    /// <summary>
    /// Entry point to app.
    /// </summary>
    public class Startup
    {

        [STAThread]
        public static void Main(string[] args) {
            int exitStatus = 0;
            Console.WriteLine("Working dir: " + Environment.CurrentDirectory);
            
            //Load foreign assemblies.
            //path from solution/project/bin/debug dir.
            //package at solution/packages dir
            Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "../../../packages/AvalonEdit.6.2.0.78/lib/net462/ICSharpCode.AvalonEdit.dll"));
           
            App application = new App();
            Console.WriteLine("Main(): application is running...");
            exitStatus = application.Run();
            Console.WriteLine("Main(): application finished with status " + exitStatus);
        }
    }
}
