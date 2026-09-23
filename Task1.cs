using System;
using System.Threading;

class Program
{
    static void Worker(object? arg)
    {
        long id = (long)arg!;
        int cpu = Thread.GetCurrentProcessorId();

        Console.WriteLine($"Thread {id}: starting (running on logical CPU {cpu})");
    }

    static void Main()
    {
        int numCores = Environment.ProcessorCount;

        Console.WriteLine($"Detected logical cores: {numCores}");

        Thread[] threads = new Thread[numCores];

        for (int i = 0; i < numCores; i++)
        {
            int idx = i; // Local copy avoids closure/shared-variable issue

            threads[i] = new Thread(() => Worker((long)idx));
            threads[i].Start();
        }

        for (int i = 0; i < numCores; i++)
        {
            threads[i].Join();
        }

        Console.WriteLine($"All {numCores} threads completed.");
    }
}