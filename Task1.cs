using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length > 0 && args[0] == "--child")
        {
            RunAsChild();
        }
        else
        {
            RunAsParent();
        }
    }

    static void RunAsChild()
    {
        Console.WriteLine($"[Child] PID = {Environment.ProcessId}");

        int counter = 100;
        counter += 50;

        Console.WriteLine($"[Child] final counter = {counter}");
    }

    static void RunAsParent()
    {
        Console.WriteLine($"[Parent] PID = {Environment.ProcessId}");

        int counter = 100;
        counter += 1;

        string processPath = Environment.ProcessPath!;

        var startInfo = new ProcessStartInfo
        {
            FileName = processPath,
            UseShellExecute = true
        };

        // When running with the .NET host, ProcessPath can be dotnet.exe.
        // In that case, pass the current DLL path before --child.
        string processName = System.IO.Path.GetFileNameWithoutExtension(processPath);

        if (processName.Equals("dotnet", StringComparison.OrdinalIgnoreCase))
        {
            startInfo.ArgumentList.Add(Environment.GetCommandLineArgs()[0]);
        }

        startInfo.ArgumentList.Add("--child");

        using Process? child = Process.Start(startInfo);

        if (child == null)
        {
            Console.WriteLine("Failed to start child process.");
            return;
        }

        child.WaitForExit();

        Console.WriteLine($"[Parent] final counter = {counter}");
        Console.WriteLine(
            "[Parent] Parent and child counters were modified independently " +
            "(separate address spaces)."
        );
    }
}