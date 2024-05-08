using System.Globalization;
using System.IO.MemoryMappedFiles;
using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions.MMFSolution;

public class MMFWorker
{
    private MemoryMappedViewAccessor memoryMappedViewAccessor;
    private readonly MemoryMappedFile mmf;
    private (long start, long end) delta;

    public MMFWorker((long start, long end) delta, MemoryMappedFile mmf)
    {
        this.memoryMappedViewAccessor = mmf.CreateViewAccessor(delta.start, delta.end - delta.start, MemoryMappedFileAccess.Read);
        this.mmf = mmf;
        this.delta = delta;
    }
    public Dictionary<string, Result> Ackern()
    {
        Dictionary<string, List<double>> measurements = new();
            
        unsafe
        {
            byte* pointer = (byte*)0;
            memoryMappedViewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
            Span<byte> span = new (pointer, (int)(delta.end - delta.start));
            fixed(byte* ptr = span)
            {
                byte* endPtr = ptr + span.Length;

                byte* startCity = ptr;
                byte* endCity = ptr;
                byte* startTmp = ptr;
                byte* endTmp = ptr;
                string cityName = string.Empty;
                
                for (byte* currentPtr = ptr; currentPtr <= endPtr; currentPtr++)
                {
                    //Gleichzeichen
                    if (*currentPtr == 61)
                    {
                        endCity = currentPtr - 1;
                        startTmp = currentPtr + 1;
                        cityName = ConvertCityName(startCity, endCity);
                        continue;
                    }

                    //Semikolon gefunden
                    if (*currentPtr == 59)
                    {
                        endTmp = currentPtr - 1;
                        startCity = currentPtr + 2;
                        
                        double tmpValue = GetTemperature(startTmp, endTmp);
                        if (measurements.ContainsKey(cityName))
                        {
                            measurements[cityName].Add(tmpValue);
                        }
                        else
                        {
                            measurements.Add(cityName, new List<double>());
                            measurements[cityName].Add(tmpValue);
                        }
                    }
                }
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
    
    private unsafe String ConvertCityName(byte* start, byte* end)
    {
        int length = (int)(end - start + 1);
        return System.Text.Encoding.UTF8.GetString(start, length);
    }

    //TODO: Hier verschiedene möglichkeiten testen: Also switch oder convert
    private unsafe double GetTemperature(byte* startTmp, byte* endTmp)
    {
        int length = (int)(endTmp - startTmp + 1);
        String tmp = System.Text.Encoding.UTF8.GetString(startTmp, length);
        return Double.Parse(tmp, CultureInfo.InvariantCulture);
    }
}