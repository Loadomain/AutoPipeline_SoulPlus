using System;
using System.Windows.Forms;

namespace BaiduAutoDownloader
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationConfiguration.Initialize();

            if (args != null && args.Length > 0 && args[0] == "--test")
            {
                if (args.Length > 2)
                    CliRunner.Run(args[1], args[2]);
                else if (args.Length > 1)
                    CliRunner.Run(args[1]);
                else
                    CliRunner.Run();
                return;
            }

            Application.Run(new MainForm());
        }
    }
}