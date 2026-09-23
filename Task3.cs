using System;
using System.Diagnostics;
using System.Threading;

class Program
{
    const int Iterations = 50;

    static void Main(string[] args)
    {
        // Child mode: do nothing and exit immediately.
        if (args.Length > 0 && args[0] == "--child")
        {
            return;
        }

        var processStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            string processPath = Environment.ProcessPath!;
            var startInfo = new ProcessStartInfo
            {
                FileName = processPath,
                UseShellExecute = true
            };

            // If launched through dotnet, pass the current DLL first.
            string processName = System.IO.Path.GetFileNameWithoutExtension(processPath);

            if (processName.Equals("dotnet", StringComparison.OrdinalIgnoreCase))
            {
                startInfo.ArgumentList.Add(Environment.GetCommandLineArgs()[0]);
            }

            startInfo.ArgumentList.Add("--child");

            using Process? child = Process.Start(startInfo);

            if (child == null)
            {
                throw new InvalidOperationException("Could not start child process.");
            }

            child.WaitForExit();
        }

        processStopwatch.Stop();

        var threadStopwatch = Stopwatch.StartNew();

        for (int i = 0; i < Iterations; i++)
        {
            Thread t = new Thread(() =>
            {
                // Trivial thread work.
            });

            t.Start();
            t.Join();
        }

        threadStopwatch.Stop();

        double avgProcessMs =
            processStopwatch.Elapsed.TotalMilliseconds / Iterations;

        double avgThreadMs =
            threadStopwatch.Elapsed.TotalMilliseconds / Iterations;

        double ratio = avgProcessMs / avgThreadMs;

        Console.WriteLine($"Iterations: {Iterations}");
        Console.WriteLine(
            $"Average process creation time: {avgProcessMs:F3} ms");
        Console.WriteLine(
            $"Average thread creation time: {avgThreadMs:F3} ms");
        Console.WriteLine(
            $"Process/Thread ratio: {ratio:F1}x");
    }
}
