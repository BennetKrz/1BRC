namespace _1BRC;
using System.Globalization;

public class SimpleLinear
{

    IEnumerable<string> lines = File.ReadLines(@"G:\C#\1BRC\1BRC\measurements.txt");
    Dictionary<string, List<Double>> measurements = new();

    public void Start()
    {
        int count = 0;

        foreach (string line in lines)
        {
            if (count++ % 10_000_000 == 0)
            {
                Console.WriteLine(count);
            }
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

        measurements = measurements.OrderBy(kvp => kvp.Key).ToDictionary();

        foreach (string city in measurements.Keys)
        {
            List<Double> values = measurements[city];
            string result = city + ": { " + values.Min() + " | avg: " + (values.Sum() / values.Count) + " | max: " +
                            values.Max() + " }";
            Console.WriteLine(result);
        }

        Console.WriteLine("Fertig");
    }
}