using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace FakeBSOD
{
    internal class Program
    {
        public static string Text = @"A problem has been detected and Windows has been shut down to prevent damageto your computer.

DRIVER_IRQL_NOT_LESS_OR_EQUAL

If this is the first time you've seen this stop error screen,restart your computer. 
If this screen appears again, follow these steps:

Check to make sure that any new hardware or software is properly installed.
If this is a new installation, ask your hardware or software manufacturerfor any windows updates you might need.

If problems continue, disable or remove any newly installed hardware or software. 
Disable BIOS memory options such as caching or shadowing.
If you need to use Safe Mode to remove or disable components, restart your computer, press F8 to select Advanced Startup Options, and then select Safe Mode.

Technical information:

*** STOP: 0x00000099 (0x00000000, 0x98A0E7D1, 0x00000008, 0xC0000000)

*** ntoskrnl.exe - Address 98A0E7D1 base at 98A0D000, DateStamp 36B0088F

";


        [STAThread]
        static int Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) => 
            {
                Exception ex = e.ExceptionObject as Exception;
                MessageBox.Show("Error:\r\n\r\n" + ex.Message, "Uncaught exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            bool loop = false;
            foreach (string arg in args)
            {
                string cmd = arg.ToLower();
                if (Regex.IsMatch(cmd, @"(-|/)mode:loop"))
                {
                    loop = true;
                }
                if (Regex.IsMatch(cmd, @"(-|/)mode:once"))
                {
                    loop = false;
                }
            }
            Timer timer = new Timer(); // 模仿老式系统逐行刷新
            Form form = new Form();
            Label text = new Label();
            timer.Interval = 1;
            timer.Tick += (sender, e) =>
            {
                if (text.Bottom < form.Height - 50)
                {
                    text.Height += 30;
                }
                else
                {
                    if (loop)
                    {
                        text.Height = 0;
                    }
                    else
                    {
                        timer.Stop();
                    }
                }
            };
            text.BackColor = Color.Transparent;
            text.Left = 50;
            text.Top = 50;
            text.Width = Screen.PrimaryScreen.Bounds.Width - 100;
            text.ForeColor = Color.White;
            text.Text = Text.Replace(" ", " \u200b");// 以单词为分割
            text.Font = new Font("Lucida Console", 20);
            form.Text = string.Empty;
            form.ShowIcon = false;
            form.ShowInTaskbar = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Left = 0;
            form.Top = 0;
            form.Width = Screen.PrimaryScreen.Bounds.Width;
            form.Height = Screen.PrimaryScreen.Bounds.Height;
            form.BackColor = Color.FromArgb(0, 0, 128);
            form.TopMost = true;
            Bitmap bitmap = new Bitmap(1, 1);
            bitmap.MakeTransparent();
            form.Cursor = new Cursor(bitmap.GetHicon());
            bitmap.Dispose();
            form.Controls.Add(text);
            form.Load += (sender, e) =>
            {
                timer.Start();
            };
            form.KeyDown += (object sender, KeyEventArgs e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    Application.Exit();
                }
            };
            form.ShowDialog();
            return 0;
        }
    }
}
