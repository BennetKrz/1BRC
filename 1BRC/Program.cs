using System.Diagnostics;
using _1BRC.Solutions;
using _1BRC.Solutions.MMFSolution;

namespace _1BRC;

public static class Program
{
    public static async Task Main(string[] args)
    {
        
        Stopwatch sw = Stopwatch.StartNew();
        AbstractManager manager = new MmfManager(12, "/home/bennet/1BRC/measurements.txt");
        await manager.Start();
        
        //SimpleLinear s = new();
        //s.Start();

        //ThreadedManager manager = new(12);
        //await manager.Start();
        //Test

        sw.Stop();

        Console.WriteLine($"{sw.Elapsed.Minutes}m:{sw.Elapsed.Seconds}s:{sw.Elapsed.Milliseconds}ms");
        Dictionary<string, List<double>> test = new();
    }
}