using System.Diagnostics;
using _1BRC;
using _1BRC.Solutions;
using _1BRC.Solutions.MMFSolution;
using _1BRC.Solutions.Simple;
using _1BRC.Solutions.ThreadedSimple;

Stopwatch sw = Stopwatch.StartNew();

MmfManager mmfManager = new(12);
await mmfManager.Start();


//SimpleLinear s = new();
//s.Start();

//ThreadedManager manager = new(12);
//await manager.Start();

sw.Stop();

Console.WriteLine($"{sw.Elapsed.Minutes}m:{sw.Elapsed.Seconds}s:{sw.Elapsed.Milliseconds}ms");