using System.Globalization;

namespace _1BRC.Solutions;

public class Threaded(long numberOfLines, int threadNumber, long extra)
{
    public Dictionary<string, Result> AnalyseLines()
    {
        Console.WriteLine("Thread " + threadNumber + " started!");
        
        Dictionary<string, List<double>> measurements = new();
        IEnumerable<string> lines = File.ReadLines(ThreadedManager.MeasurementsFile)
                                        .Skip((int)(threadNumber * numberOfLines))
                                        .Take((int)(numberOfLines + extra));
        
        int count = 0;
        foreach (string line in lines)
        {
            if (count != 0 && count % 10_000_000 == 0)
            {
                Console.WriteLine(count + " lines on thread: " + threadNumber);
            }

            count++;
            string[] data = line.Split(";");
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

        Console.WriteLine("Thread " + threadNumber + " finished!");
        return results;
    }
}