using System;

namespace _1BRC;

public class ThreadedManager(int threadCount = 6)
{
    private List<Task<Dictionary<string, Result>>> threads = new();
    private List<Dictionary<string, Result>> results = new();
    
    public async Task Start()
    {
        int linesPerThread = 166_666_666;
        for (int i = 0; i < threadCount; i++)
        {
            int extra = 0;
            if (i == threadCount - 1)
            {
                extra = 4;
            }

            int ii = i;
            
            Task<Dictionary<string, Result>> t =
                Task.Run(() => new Threaded(linesPerThread, ii, extra).analyseLines());
            threads.Add(t);
            
        }

        await Task.WhenAll(threads);

        for (int i = 0; i < threads.Count; i++)
        {
            results.Add(threads[i].Result);
        }

        Dictionary<string, List<Result>> tmp = new();
        
        for (int i = 0; i < results.Count; i++)
        {
            Dictionary<string, Result> result = results[i];
            foreach (string key in result.Keys)
            {
                if (tmp.ContainsKey(key))
                {
                    tmp[key].Add(result[key]);
                }
                else
                {
                    tmp.Add(key, new List<Result>());
                    tmp[key].Add(result[key]);
                }
            }
        }

        tmp = tmp.OrderBy(r => r.Key).ToDictionary();
        
        foreach (string city in tmp.Keys)
        {
            List<Result> allResultsFromThread = tmp[city];
            double min = allResultsFromThread.Min(r => r.Min);
            double max = allResultsFromThread.Max(r => r.Max);
            double avg = allResultsFromThread.Sum(r => r.Sum) / allResultsFromThread.Count;

            Console.WriteLine(city + ": { " + min + " | " + avg + " | " + max + " }");
        }
    }
}