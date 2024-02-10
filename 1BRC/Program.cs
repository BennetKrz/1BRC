using System.Diagnostics;
using _1BRC;
using _1BRC.Solutions;

Stopwatch sw = Stopwatch.StartNew();

ThreadedManager manager = new();
await manager.Start();

sw.Stop();

Console.WriteLine($"{sw.Elapsed.Minutes}m:{sw.Elapsed.Seconds}s:{sw.Elapsed.Milliseconds}ms");