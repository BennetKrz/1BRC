using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions;

public interface IWorker
{
    public Dictionary<string, Result> AnalyseLines();
}