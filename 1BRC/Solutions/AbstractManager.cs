using System.Text;

namespace _1BRC.Solutions;

public abstract class AbstractManager
{
    public async Task Start()
    {
        List<Task<Dictionary<string, Result>>> threads = StarThreads();
        await Task.WhenAll(threads);

        List<Dictionary<string, Result>> results = threads.Select(t => t.Result)
            .ToList();

        Dictionary<string, List<Result>> resultsPerCity = GetResultsPerCity(results);
        resultsPerCity = resultsPerCity.OrderBy(r => r.Key).ToDictionary();

        CalculateOutput(resultsPerCity);
    }

    private static void CalculateOutput(Dictionary<string, List<Result>> resultsPerCity)
    {
        StringBuilder sb = new();
        
        foreach (string city in resultsPerCity.Keys)
        {
            List<Result> allResultsFromThread = resultsPerCity[city];
            double min = allResultsFromThread.Min(r => r.Min);
            double max = allResultsFromThread.Max(r => r.Max);
            double avg = allResultsFromThread.Sum(r => r.Sum) / allResultsFromThread.Count;
            min = double.Round(min, 1);
            avg = double.Round(avg, 1);
            max = double.Round(max, 1);

            sb.Append($"{city}={min}/{avg}/{max}\n");
        }

        Console.WriteLine(sb.ToString());
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

    protected abstract List<Task<Dictionary<string, Result>>> StarThreads();
}