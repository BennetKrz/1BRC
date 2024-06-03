using System.Diagnostics;
using _1BRC.Solutions;
using _1BRC.Solutions.MMFSolution;

namespace _1BRC;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (args.Length > 0 && File.Exists(args[0]))
        {
            AbstractManager.filePath = args[0];
        }
        
        Stopwatch sw = Stopwatch.StartNew();
        MmfManager mmfManager = new(12);
        await mmfManager.Start();
        
        //SimpleLinear s = new();
        //s.Start();

        //ThreadedManager manager = new(12);
        //await manager.Start();

        sw.Stop();

        Console.WriteLine($"{sw.Elapsed.Minutes}m:{sw.Elapsed.Seconds}s:{sw.Elapsed.Milliseconds}ms");
        Dictionary<string, List<double>> test = new();
    }
}