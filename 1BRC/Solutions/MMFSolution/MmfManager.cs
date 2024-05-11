using System.IO.MemoryMappedFiles;
using System.Text;
using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions.MMFSolution;

public class MmfManager : AbstractManager
{
    private readonly MemoryMappedFile mmf;
    private readonly MemoryMappedViewAccessor memoryMappedViewAccessor;

    public MmfManager(int threadCount) : base(threadCount)
    {
        this.mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, "test");
        memoryMappedViewAccessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
    }

    protected override List<Task<Dictionary<string, Result>>> StarThreads(List<(long start, long end)> offsets)
    {
        List<Task<Dictionary<string, Result>>> threads = new();

        foreach ((long start, long end) offset in offsets)
        {
            Task<Dictionary<string, Result>> t =
                Task.Run(() => new MmfWorker(offset, this.memoryMappedViewAccessor).AnalyseLines());
            threads.Add(t);
        }

        return threads;
    }

    protected override List<(long start, long end)> CalculateOffsets()
    {
        List<(long start, long end)> offsets = new();

        FileInfo info = new(filePath);
        long fileSize = info.Length;

        long offsetEach = fileSize / this.threadCount;
        long curPos = 0;
        long begin = 0;

        for (int i = 0; i < this.threadCount; i++)
        {
            curPos += offsetEach;
            if (i == this.threadCount - 1)
            {
                offsets.Add((begin, fileSize));
                break;
            }

            while (memoryMappedViewAccessor.ReadByte(curPos) != 59)
            {
                curPos++;
            }

            byte b = memoryMappedViewAccessor.ReadByte(curPos);

            long end = curPos;
            (long start, long end) delta = (begin, end);
            offsets.Add(delta);
            begin = curPos + 2L; //; und \n überspringen
        }

        return offsets;
    }
}