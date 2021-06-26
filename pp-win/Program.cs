using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pp_win
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Test();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void Test()
        {
            var rdr = File(
@"void foo() {if (a = 3 && b = 3)
{
print(x);
print(2);
}}");
            var parser = new Parser(rdr);
            var cu = parser.Parse();
            var writer = new StringWriter();
            var device = new OutputDevice(writer, 12, 3);
            var pp = new PrettyPrinter(device);
            
            var cur = new CompilationUnitRenderer(pp);
            cur.Render(cu);
            Debug.WriteLine(writer.ToString());
        }

        private static TextReader File(string v)
        {
            return new StringReader(v);
        }

        private static void Render(PrettyPrinter pp)
        {
        }
    }
}
