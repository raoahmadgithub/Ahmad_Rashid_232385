using System;
using System.Threading;

class Program
{
    const int NumThreads = 4;
    const int IncrementsPerThread = 250_000;

    static int counter = 0;

    // Task 3 mein --lock mode ke liye
    static readonly object gate = new object();

    static bool useLock = false;

    // Har thread ka apna private log
    static int[][] seenLog = new int[NumThreads][];

    static void Worker(int id)
    {
        int[] log = seenLog[id];

        for (int i = 0; i < IncrementsPerThread; i++)
        {
            if (useLock)
            {
                lock (gate)
                {
                    int seen = counter;   // Load
                    log[i] = seen;        // Record loaded value
                    counter = seen + 1;   // Add and store
                }
            }
            else
            {
                int seen = counter;       // Load
                log[i] = seen;            // Record loaded value
                counter = seen + 1;       // Add and store
            }
        }
    }

    static void Main(string[] args)
    {
        useLock = args.Length > 0 && args[0] == "--lock";

        int total = NumThreads * IncrementsPerThread;

        Thread[] threads = new Thread[NumThreads];

        // Create and start threads
        for (int t = 0; t < NumThreads; t++)
        {
            seenLog[t] = new int[IncrementsPerThread];

            int id = t;

            threads[t] = new Thread(() => Worker(id));
            threads[t].Start();
        }

        // Wait for all threads
        foreach (Thread th in threads)
        {
            th.Join();
        }

        // Count how many times each value was loaded
        int[] readCount = new int[total + 1];

        for (int t = 0; t < NumThreads; t++)
        {
            for (int i = 0; i < IncrementsPerThread; i++)
            {
                readCount[seenLog[t][i]]++;
            }
        }

        // Count collisions
        int collisions = 0;

        for (int v = 0; v <= total; v++)
        {
            if (readCount[v] > 1)
            {
                collisions += readCount[v] - 1;
            }
        }

        string mode = useLock ? "with lock" : "no synchronization";

        Console.WriteLine("Task 2: Race Trace");
        Console.WriteLine("--------------------------------");
        Console.WriteLine($"Mode: {mode}");
        Console.WriteLine($"Total increments: {total}");
        Console.WriteLine($"Final counter: {counter}");
        Console.WriteLine($"Lost updates: {total - counter}");
        Console.WriteLine($"Collisions: {collisions}");

        // Print first 5 colliding values
        Console.WriteLine();
        Console.WriteLine("First 5 Colliding Values:");
        Console.WriteLine("--------------------------------");

        int printed = 0;

        for (int v = 0; v <= total && printed < 5; v++)
        {
            if (readCount[v] > 1)
            {
                Console.WriteLine($"\nCollision Value: {v}");

                // Search all thread logs
                for (int t = 0; t < NumThreads; t++)
                {
                    for (int i = 0; i < IncrementsPerThread; i++)
                    {
                        if (seenLog[t][i] == v)
                        {
                            Console.WriteLine(
                                $"Thread {t} loaded {v} at loop index {i}"
                            );
                        }
                    }
                }

                printed++;
            }
        }

        if (printed == 0)
        {
            Console.WriteLine("No collisions found.");
        }
    }
}