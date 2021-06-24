using Reko.Core.Output;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace pp_win
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtSrc.Text = @"if (%{a == 3 && b == 3%})%n
{%t%n
print(""#"");%n
print('2');% n
% b}%n
";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        private void UpdateOutput()
        {
            var (deviceWidth, indentWidth) = GetDeviceParameters();
            if (!(deviceWidth.HasValue && indentWidth.HasValue))
                return;
            var sw = new StringWriter();
            var output = new OutputDevice(sw, deviceWidth.Value, indentWidth.Value);
            var pp = new PrettyPrinter(output);
            pp.prettyprint(this.txtSrc.Text);
            txtOutput.Text = sw.ToString();
        }

        private (int?, int?) GetDeviceParameters()
        {
            if (!int.TryParse(txtDeviceWidth.Text, out int deviceWidth))
                return (null, null);
            if (!int.TryParse(txtIndentWidth.Text, out int indentWidth))
                return (null, null);
            return (deviceWidth, indentWidth);
        }

        private void txtDeviceWidth_TextChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }

        private void txtIndentWidth_TextChanged(object sender, EventArgs e)
        {
            UpdateOutput();
        }
    }
}
