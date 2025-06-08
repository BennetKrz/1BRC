using System.Globalization;

namespace _1BRC.Solutions.ThreadedSimple;

public class ThreadedWorker(long numberOfLines, int threadNumber, long extra, string filePath) : IWorker
{
    public Dictionary<string, Result> AnalyseLines()
    {
        Console.WriteLine("Thread " + threadNumber + " started!");
        
        Dictionary<string, List<double>> measurements = new();
        IEnumerable<string> lines = File.ReadLines(filePath)
                                        .Skip((int)(threadNumber * numberOfLines))
                                        .Take((int)(numberOfLines + extra));
        
        foreach (string line in lines)
        {
            string[] data = line.Replace(";","").Split("=");
            if (!measurements.ContainsKey(data[0]))
            {
                measurements.Add(data[0], new List<double>());
                measurements[data[0]].Add(double.Parse(data[1], CultureInfo.InvariantCulture));
            } 
            else 
            {
                measurements[data[0]].Add(double.Parse(data[1], CultureInfo.InvariantCulture));
            }
        }

        Dictionary<string, Result> results = new();
        foreach (string city in measurements.Keys)
        {
            List<double> values = measurements[city];
            results.Add(city, new Result(values.Min(), values.Sum() / values.Count(), values.Max()));
        }
        
        return results;
    }
}