using System;
using System.IO;

public class OutputDevice
{
    private readonly TextWriter writer;
    private bool atStart;

    public OutputDevice(TextWriter writer, int deviceWidth, int indentWidth)
    {
        this.writer = writer;
        this.DeviceWidth = deviceWidth;
        this.IndentWidth = indentWidth;
        this.atStart = true;
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
        writer.WriteLine();
        atStart = true;
    }

    internal void Outdent()
    {
        LeftMargin -= IndentWidth;
    }

    internal void Indent()
    {
        LeftMargin += IndentWidth;
    }
}