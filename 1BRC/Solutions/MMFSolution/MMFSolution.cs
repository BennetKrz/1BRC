using System.IO.MemoryMappedFiles;
using System.Text;
using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions.MMFSolution;

public class MMFSolution
{
    private readonly string filePath = @"H:\Java\1brc-main\measurements.txt";
    private readonly int threadCount;
    private readonly MemoryMappedFile mmf;

    public MMFSolution(int threadCount)
    {
        this.mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, "test");
        this.threadCount = threadCount;
    }

    public async Task Start()
    {
        List<(long start, long end)> offsets = CalculateOffsets();
        
        List<Task<Dictionary<string, Result>>> threads = StarThreads(offsets);
        
        await Task.WhenAll(threads);

        List<Dictionary<string, Result>> results = threads.Select(t => t.Result)
            .ToList();
        
        Dictionary<string, List<Result>> resultsPerCity = GetResultsPerCity(results);
        resultsPerCity = resultsPerCity.OrderBy(r => r.Key).ToDictionary();
        
        foreach (string city in resultsPerCity.Keys)
        {
            List<Result> allResultsFromThread = resultsPerCity[city];
            double min = allResultsFromThread.Min(r => r.Min);
            double max = allResultsFromThread.Max(r => r.Max);
            double avg = allResultsFromThread.Sum(r => r.Sum) / allResultsFromThread.Count;
            avg = double.Round(avg, 1);

            Console.WriteLine(city + ": { " + min + " | " + avg + " | " + max + " }");
        }
        
        int debug = 0;
    }
    
    private Dictionary<string, List<Result>> GetResultsPerCity(List<Dictionary<string, Result>> results)
    {
        Dictionary<string, List<Result>> resultsPerCity = new();
        foreach (Dictionary<string, Result> result in results)
        {
            foreach (string key in result.Keys)
            {
                if (resultsPerCity.ContainsKey(key))
                {
                    resultsPerCity[key].Add(result[key]);
                }
                else
                {
                    resultsPerCity.Add(key, new List<Result>());
                    resultsPerCity[key].Add(result[key]);
                }
            }
        }
        return resultsPerCity;
    }

    private List<Task<Dictionary<string, Result>>> StarThreads(List<(long start, long end)> offsets)
    {
        List<Task<Dictionary<string, Result>>> threads = new();

        foreach ((long start, long end) offset in offsets)
        {
            Task<Dictionary<string, Result>> t =
                Task.Run(() => new MMFWorker(offset, this.mmf).Ackern());
            threads.Add(t);
        }
        
        return threads;
    }
    
    private List<(long start, long end)> CalculateOffsets()
    {
        List<(long start, long end)> offsets = new(); 
        
        FileInfo info = new(filePath);
        long fileSize = info.Length;
        
        MemoryMappedViewAccessor memoryMappedViewAccessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        
        long offsetEach = fileSize / this.threadCount;
        long curPosition = offsetEach;

        long begin = 0;
        
        for (int i = 0; i < this.threadCount; i++)
        {
            if (i == this.threadCount - 1)
            {
                offsets.Add((begin, fileSize));
                break;
            }
            while (memoryMappedViewAccessor.ReadByte(curPosition) != 59)
            {
                curPosition++;
            }

            (long start, long end) delta = (begin, curPosition++);
            offsets.Add(delta);
            begin = curPosition;
            curPosition += offsetEach;
        }

        return offsets;
    }
}