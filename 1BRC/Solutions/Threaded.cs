using System.Globalization;

namespace _1BRC;

public class Threaded(int numberOfLines, int threadNumber, int extra)
{
    public Dictionary<string, Result> analyseLines()
    {
        Console.WriteLine("Thread " + threadNumber + " started!");
        Dictionary<string, List<double>> measurements = new();
        int count = 0;
        IEnumerable<string> lines = File.ReadLines(@"G:\C#\1BRC\1BRC\measurements.txt")
                                        .Skip(threadNumber * numberOfLines)
                                        .Take(numberOfLines + extra);
        foreach (string line in lines)
        {
            if (count % 10_000_000 == 0)
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