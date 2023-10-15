using System;
using System.Reflection;
using System.Windows.Forms;

namespace RawInputReader
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Icon = Resources.RawInputReader_icon;
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private void AboutRawInputReaderToolStripMenuItem_Click(object sender, System.EventArgs e)
        {
            var asm = Assembly.GetEntryAssembly()!;
            MessageBox.Show(
                asm.GetCustomAttribute<AssemblyTitleAttribute>()!.Title + " - " + (IntPtr.Size == 4 ? "32" : "64") + "-bit" + Environment.NewLine + asm.GetCustomAttribute<AssemblyCopyrightAttribute>()!.Copyright,
                asm.GetCustomAttribute<AssemblyTitleAttribute>()!.Title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}