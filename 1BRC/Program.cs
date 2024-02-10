using System.Diagnostics;
using _1BRC;
using _1BRC.Solutions;
using _1BRC.Solutions.ThreadedSimple;

Stopwatch sw = Stopwatch.StartNew();

ThreadedManager manager = new(6);
await manager.Start();

sw.Stop();

Console.WriteLine($"{sw.Elapsed.Minutes}m:{sw.Elapsed.Seconds}s:{sw.Elapsed.Milliseconds}ms");