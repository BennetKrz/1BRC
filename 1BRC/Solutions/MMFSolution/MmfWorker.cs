using System.Globalization;
using System.IO.MemoryMappedFiles;
using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions.MMFSolution;

public class MmfWorker((long start, long end) delta, MemoryMappedViewAccessor memoryMappedViewAccessor)
    : IWorker
{
    public Dictionary<string, Result> AnalyseLines()
    {
        Dictionary<string, List<int>> measurements = [];
        
        unsafe
        {
            byte* pointer = (byte*)0;
            memoryMappedViewAccessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);
            Span<byte> span = new (pointer + delta.start, (int)(delta.end - delta.start));
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
                    if (*currentPtr != 59) continue;
                    
                    endTmp = currentPtr - 1;
                    startCity = currentPtr + 2;
                        
                    int tmpValue = GetTemperature(startTmp, endTmp);
                    if (!measurements.ContainsKey(cityName))
                    {
                        measurements.Add(cityName, new List<int>(205_000));
                    }
                    measurements[cityName].Add(tmpValue);
                }
            }
        }
        
        Dictionary<string, Result> results = new();
        foreach (string city in measurements.Keys)
        {
            List<int> values = measurements[city];
            results.Add(city, new Result(values.Min() * 0.1, ((double)values.Sum() / (double)values.Count) * 0.1, values.Max() * 0.1));
        }
        
        return results;
    }
    
    private unsafe String ConvertCityName(byte* start, byte* end)
    {
        int length = (int)(end - start + 1);
        string cityName = System.Text.Encoding.UTF8.GetString(start, length);
        return cityName;
    }

    //TODO: Hier verschiedene möglichkeiten testen: Also switch oder convert
    private unsafe int GetTemperature(byte* startTmp, byte* endTmp)
    {
        int length = (int)(endTmp - startTmp + 1);
        return ConvertByteToDigit(startTmp, length);
    }

    private unsafe int ConvertByteToDigit(byte* startTmp, int length)
    {
        int tmp = 0;
        int pos = 1;
        bool isNegative = false;
        int start = 0;
        if (*startTmp == 45)
        {
            start++;
            isNegative = true;
        }
        for (int i = length - 1; i >= start; i--)
        {
            byte t = *(startTmp + i);
            if (t == 46)
            {
                continue;
            }

            tmp += pos * (t - 48);
            pos *= 10;
        }
        return isNegative ? tmp * -1 : tmp;
    }
}