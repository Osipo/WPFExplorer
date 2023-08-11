using DevExpress.Internal.WinApi.Windows.UI.Notifications;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WpfExplorer.Models;
using WpfExplorer.Models.ML.Networks.Optimizers;
using WpfExplorer.Models.ML.Networks;
using WpfExplorerControl.Extensions;
using System.Windows.Media.Converters;
using System.Media;
using System.Collections.Specialized;
using WpfExplorerControl.Utils;

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

            Console.WriteLine(new int[] {22, 33, 33, 22}.AsEnumerable().Enumerate().ToString<int>());

            Stopwatch sw = new Stopwatch();
            IEnumerable<decimal> t1 = new decimal[] { 1M, 2M, 1M, 0M, -2M, -2M, 3M, -4M, 5M, 5.11M, 5.11M, -4.11M, -4.01M, 6.01M, 6M }.AsEnumerable();
            (decimal max, int index) = (0M, -1);
            sw.Start();
            (max, index) = (t1.MaxElem(), t1.MaxIndex());
            sw.Stop();
            Console.WriteLine($"Found Max {max} as {index} element for {sw.ElapsedMilliseconds} mills.");
            sw.Reset();
            sw.Start();
            (max, index) = t1.MaxWithIndex();
            sw.Stop();
            Console.WriteLine($"Found Max {max} as {index} element for {sw.ElapsedMilliseconds} mills.");

            Console.WriteLine(StaticObjects.Sigmoid.Fn(new[] { 0.25, 0.5, 0.75, 1, 2, 3, 4 }).ToString<double>()); ;
            Console.WriteLine(new[] { 1.1, 2.2, 1.1 }.Union(new[] {1.1}).ToString<double>());
            Console.WriteLine(new[] {1, 2, 3 }.Union(new[] { 2 }, new IgnoreEqualityComparer<int>()).ToString<int>());

           

            var set = new System.Collections.Generic.SortedSet<int>(new LIFOComparer<int>());
            var set2 = new System.Collections.Generic.HashSet<int>(new IgnoreEqualityComparer<int>());
            for(int i = 0; i < 10; i++)
            {
                set.Add(i);
                set2.Add(i);
                set2.Add(i);
            }
            Console.WriteLine(set.ToString<int>());
            Console.WriteLine(set2.ToString<int>());

            //SHOW NETWORK
            //TestOrNeuron();

            //Load foreign assemblies.
            //path from solution/project/bin/debug dir.
            //package at solution/packages dir
            //Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "../../../packages/AvalonEdit.6.2.0.78/lib/net462/ICSharpCode.AvalonEdit.dll"));
            //Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "../../../packages/Mono.Cecil.0.10.0/lib/net40/Mono.Cecil.dll"));
            //Assembly.LoadFrom(Path.Combine(Environment.CurrentDirectory, "../../../packages/ICSharpCode.Decompiler.3.1.0.3652/lib/net46/ICSharpCode.Decompiler.dll"));

            App application = new App();
            Console.WriteLine("Main(): application is running...");
            exitStatus = application.Run();
            Console.WriteLine("Main(): application finished with status " + exitStatus);
        }

        private static void TestOrNeuron()
        {
            NeuralNetwork network = new NetworkBuilder(2)
                .AddLayer(new Layer(2, StaticObjects.Sigmoid, new VectorRow(new double[] { 0.25, 0.45 })))
                .AddLayer(new Layer(1, StaticObjects.Sigmoid, new VectorRow(new double[] { 0.15 })))
                .SetCostFunction(new MSE())
                .SetInitializer(WeightInitializers.XavierUniform)
                .SetOptimizer(new GradientDescent(0.1))
                .create();


            /*
            double[][] trainInputs = new double[][]{
                new []{0D, 0D},
                new []{0D, 1D},
                new []{1D, 0D},
                new []{1D, 1D}
            };

            double[][] trainOutput = new double[][]{
                new[] {0D},
                new[] {1D},
                new[] {1D},
                new[] {1D}
            };*/
            VectorRow[] trainInputs = new VectorRow[]
            {
                new VectorRow(new double[]{0D, 0D}),
                new VectorRow(new double[]{0D, 1D}),
                new VectorRow(new double[]{1D, 0D}),
                new VectorRow(new double[]{1D, 1D})
            };
            VectorRow[] trainOutput = new VectorRow[] {
                new VectorRow(new double[]{0D }),
                new VectorRow(new double[]{1D }),
                new VectorRow(new double[]{1D }),
                new VectorRow(new double[]{1D })
            };

            //обучение
            int cnt = 0;
            for (int i = 0; i < 55500; i++)
            {
                //Console.WriteLine(i);
                IEnumerable<double> input = trainInputs[cnt];
                IEnumerable<double> expected = trainOutput[cnt];
                network.Eval(input, expected);
                network.UpdateFromLearning();
                cnt = (cnt + 1) % trainInputs.Length;
            }

            //вывод
            for (int i = 0; i < trainInputs.Length; i++)
            {
                IEnumerable<double> r = network.Eval(trainInputs[i], trainOutput[i]);
                Console.WriteLine(trainInputs[i].ToString<double>() + " ( " + trainOutput[i].ToString<double>() + " ) = " + r.ToString<double>() + " Cost: " + r.Last().ToString());
            }
        }
    }
}
