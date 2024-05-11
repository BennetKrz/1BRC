namespace _1BRC.Solutions.ThreadedSimple;

public class ThreadedManager : AbstractManager
{

    private readonly long linesInFile;
    
    public ThreadedManager(int threadCount = 6, long linesInFile = 1_000_000_000) : base(threadCount)
    {
        this.linesInFile = linesInFile;
    }
    
    protected override List<Task<Dictionary<string, Result>>> StarThreads(List<(long start, long end)> offsets)
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
                Task.Run(() => new ThreadedWorker(linesPerThread, ii, extra).AnalyseLines());
            threads.Add(t);
            
        }

        return threads;
    }

    protected override List<(long start, long end)> CalculateOffsets()
    {
        return null;
    }
}