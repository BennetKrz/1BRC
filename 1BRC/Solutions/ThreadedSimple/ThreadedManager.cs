namespace _1BRC.Solutions.ThreadedSimple;

public class ThreadedManager(
    int threadCount = 12,
    string fileName = "~/1BRC/measurements.txt",
    long linesInFile = 1_000_000_000)
    : AbstractManager
{
    protected override List<Task<Dictionary<string, Result>>> StarThreads()
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
                Task.Run(() => new ThreadedWorker(linesPerThread, ii, extra, fileName).AnalyseLines());
            threads.Add(t);
            
        }

        return threads;
    }
}