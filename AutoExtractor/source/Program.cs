using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AutoExtractor;

internal static class Program
{
    [DllImport("kernel32.dll")]
    static extern bool AttachConsole(int dwProcessId);
    private const int ATTACH_PARENT_PROCESS = -1;

    [STAThread]
    static void Main(string[] args)
    {
        if (args.Length > 0)
        {
            AttachConsole(ATTACH_PARENT_PROCESS);
            Console.WriteLine();
            RunCli(args);
        }
        else
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new Form1());
        }
    }

    static void RunCli(string[] args)
    {
        var options = new ExtractorCoreOptions
        {
            SilentMode = true // By default, CLI is silent according to requirement
        };

        for (int i = 0; i < args.Length; i++)
        {
            var arg = args[i].ToLowerInvariant();
            if (arg == "--dir" && i + 1 < args.Length)
            {
                options.RootDirectory = args[++i];
            }
            else if (arg == "--fake-ext" && i + 1 < args.Length)
            {
                options.FakeExtension = args[++i];
            }
            else if (arg == "--target-ext" && i + 1 < args.Length)
            {
                options.TargetExtension = args[++i];
            }
            else if (arg == "--password" && i + 1 < args.Length)
            {
                options.Passwords.Add(args[++i]);
            }
            else if (arg == "--help" || arg == "-h")
            {
                PrintHelp();
                Environment.Exit(0);
            }
        }

        if (string.IsNullOrWhiteSpace(options.RootDirectory) || !Directory.Exists(options.RootDirectory))
        {
            Console.WriteLine("Error: Valid --dir is required.");
            PrintHelp();
            Environment.Exit(1);
        }

        if (string.IsNullOrWhiteSpace(options.TargetExtension))
        {
            Console.WriteLine("Error: Valid --target-ext is required. Example: --target-ext .zip");
            Environment.Exit(1);
        }

        Console.WriteLine($"Starting AutoExtractor CLI Mode on {options.RootDirectory}");
        Console.WriteLine($"Target Extension: {options.TargetExtension}");
        Console.WriteLine($"Fake Extension: {(string.IsNullOrEmpty(options.FakeExtension) ? "<none>" : options.FakeExtension)}");
        Console.WriteLine($"Configured Passwords: {options.Passwords.Count}");

        var core = new ExtractorCore(options);
        core.OnLog += msg => Console.WriteLine(msg);

        // Core runs synchronously in console mode
        try
        {
            core.Run();
            Console.WriteLine("Done.");
            Environment.Exit(0);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal Error: {ex.Message}");
            Environment.Exit(2);
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine("AutoExtractor CLI Manual");
        Console.WriteLine("Usage:");
        Console.WriteLine("  AutoExtractor.exe [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --dir <path>          (Required) The root directory to scan.");
        Console.WriteLine("  --fake-ext <ext>      (Optional) Specific false extension to target (e.g. .mp4). If omitted, targets extensionless files.");
        Console.WriteLine("  --target-ext <ext>    (Required) The real extension to apply (e.g. .zip, .rar, .7z).");
        Console.WriteLine("  --password <pw>       (Optional) Add a password to try. Can be used multiple times: --password pw1 --password pw2");
        Console.WriteLine("  --help, -h            Show this help message.");
    }
}