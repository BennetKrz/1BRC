namespace _1BRC.Solutions.ThreadedSimple;

public class ThreadedManager(int threadCount = 6, long linesInFile = 1_000_000_000)
{
    public static string MeasurementsFile = @"G:\C#\1BRC\1BRC\measurements.txt";
    
    
    public async Task Start()
    {
        List<Task<Dictionary<string, Result>>> threads = StarThreads();
        
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

    private List<Task<Dictionary<string, Result>>> StarThreads()
    {
        List<Task<Dictionary<string, Result>>> threads = new();
        
        long linesPerThread = linesInFile / threadCount;
        long externalExtra = linesPerThread * threadCount == linesInFile
            ? 0
            : linesInFile - linesPerThread * threadCount;
        
        for (int i = 0; i < threadCount; i++)
        {
            long extra = 0;
            if (i == threadCount - 1)
            {
                extra = externalExtra;
            }

            int ii = i;
            
            Task<Dictionary<string, Result>> t =
                Task.Run(() => new Threaded(linesPerThread, ii, extra).AnalyseLines());
            threads.Add(t);
            
        }
        return threads;
    }
}