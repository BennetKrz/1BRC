using System.IO.MemoryMappedFiles;
using _1BRC.Solutions.ThreadedSimple;

namespace _1BRC.Solutions.MMFSolution;

public class MmfManager : AbstractManager
{
    private readonly MemoryMappedViewAccessor _memoryMappedViewAccessor;
    private readonly string _filePath;
    private readonly int _threadCount;
    
    public MmfManager(int threadCount, string filePath)
    {
        MemoryMappedFile mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open);
        _memoryMappedViewAccessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        _filePath = filePath;
        _threadCount = threadCount;
    }

    protected override List<Task<Dictionary<string, Result>>> StarThreads()
    {
        List<(long start, long end)> offsets = CalculateOffsets();
        List<Task<Dictionary<string, Result>>> threads = new();

        foreach ((long start, long end) offset in offsets)
        {
            Task<Dictionary<string, Result>> t =
                Task.Run(() => new MmfWorker(offset, _memoryMappedViewAccessor).AnalyseLines());
            threads.Add(t);
        }

        return threads;
    }

    private List<(long start, long end)> CalculateOffsets()
    {
        List<(long start, long end)> offsets = new();

        FileInfo info = new(_filePath);
        long fileSize = info.Length;

        long offsetEach = fileSize / _threadCount;
        long curPos = 0;
        long begin = 0;

        for (int i = 0; i < _threadCount; i++)
        {
            curPos += offsetEach;
            if (i == _threadCount - 1)
            {
                offsets.Add((begin, fileSize));
                break;
            }

            while (_memoryMappedViewAccessor.ReadByte(curPos) != 59)
            {
                curPos++;
            }

            byte b = _memoryMappedViewAccessor.ReadByte(curPos);

            long end = curPos;
            (long start, long end) delta = (begin, end);
            offsets.Add(delta);
            begin = curPos + 2L; //; und \n überspringen
        }

        return offsets;
    }
}