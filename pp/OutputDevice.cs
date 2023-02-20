using System;
using System.IO;

namespace Reko.Core.Output
{
    /// <summary>
    /// Abstracts the notion of an output device.
    /// </summary>
    public class OutputDevice
    {
        private readonly TextWriter writer;
        private bool atStart;

        public OutputDevice(TextWriter writer, int deviceWidth, int indentWidth)
        {
            this.writer = writer;
            DeviceWidth = deviceWidth;
            IndentWidth = indentWidth;
            atStart = true;
        }


        public int DeviceWidth { get; }
        public int IndentWidth { get; }

        public int LeftMargin { get; private set; }
        public void Write(char ch)
        {
            if (atStart)
            {
                for (int i = 0; i < LeftMargin; ++i)
                {
                    writer.Write(' ');
                }
                atStart = false;
            }
            writer.Write(ch);
        }

        public void WriteLine()
        {
            DoWriteLine();
            atStart = true;
        }

        protected virtual void DoWriteLine()
        {
            writer.WriteLine();
        }

        public void Outdent()
        {
            LeftMargin -= IndentWidth;
        }

        public void Indent()
        {
            LeftMargin += IndentWidth;
        }
    }
}