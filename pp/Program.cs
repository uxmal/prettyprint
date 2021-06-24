using Reko.Core.Output;
using System;

namespace pp
{
    class Program
    {
        static void Main(string[] args)
        {
            var device = new OutputDevice(Console.Out, 12, 3);
            var pp = new PrettyPrinter(device);
            pp.prettyprint("if (%ta_very_long_identifier%b)%n{%n%thello();%n%b}%n");
        }
    }
}
