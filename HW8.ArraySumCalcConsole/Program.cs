// See https://aka.ms/new-console-template for more information

using HW8.ArraySumCalcConsole;
using ServiceStack.Text;
using System.Collections.Concurrent;
using System.Diagnostics;

List<TaskInfo> taskInfos = new List<TaskInfo>();

List<int> treadsUsingCountList = new List<int> { 4, 8, 12, 16 };
List<int> array1HT = GenerateArray(100000);
List<int> array1M = GenerateArray(1000000);
List<int> array10M = GenerateArray(10000000);
Console.WriteLine("==============================================================");
CalcArraySync(array1HT);
CalcArraySync(array1M);
CalcArraySync(array10M);
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    await CalcArrayParallelByThreads(array1HT, tc);
}
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    await CalcArrayParallelByThreads(array1M, tc);
}
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    await CalcArrayParallelByThreads(array10M, tc);
}
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    CalcArrayParallelByParallel(array1HT, tc);
}
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    CalcArrayParallelByParallel(array1M, tc);
}
Console.WriteLine("==============================================================");
foreach (int tc in treadsUsingCountList)
{
    CalcArrayParallelByParallel(array10M, tc);
}
SaveResultsInFile();
Console.ReadKey();


List<int> GenerateArray(int elementsCount)
{
    List<int> resultList = new List<int>();
    Random rnd = new Random();
    for (int i = 0; i <= elementsCount - 1; i++)
    {
        resultList.Add(rnd.Next(0, 100));
    }
    return resultList;
}

long CalcSum(List<int> inputArray)
{
    long result = 0;
    foreach (int element in inputArray)
    {
        result += element;
    }
    return result;
}
List<List<int>> SplitArray(int[] source, int count)
{
    List<List<int>> result = new List<List<int>>();
    var totalLength = source.Count();
    var chunkLength = (int)Math.Ceiling(totalLength / (double)count);
    for (int i = 0; i < count; i++)
    {
        result.Add(source.Skip(i * chunkLength).Take(chunkLength).ToList());
    }
    return result;
}
void CalcArraySync(List<int> inputArray)
{
    Stopwatch sp = new Stopwatch();
    sp.Start();
    long sum = CalcSum(inputArray);
    sp.Stop();
    double calcingtime = sp.Elapsed.TotalMilliseconds;
    Console.WriteLine($"Сумма массива из {inputArray.Count} элементов синхронно рассчиталась за {calcingtime} милисекунд и равна {sum}");
    taskInfos.Add(new TaskInfo { ItemsCount = inputArray.Count, CalcTimeInMs = calcingtime, CalcType = "Synchroniusly", TreadsCount = 1, TotalSum = sum });
}
void CalcArrayParallelByThreads(List<int> inputArray, int threadsCount)
{
    long sum = 0;
    var intsArrayList = SplitArray(inputArray.ToArray(), threadsCount);
    ConcurrentBag<long> resList = new ConcurrentBag<long>();
    Stopwatch sp = new Stopwatch();
    sp.Start();
    Parallel.ForEach(intsArrayList, t => resList.Add(CalcSum(t)));
    sp.Stop();
    sum = resList.Sum();
    double calcingtime = sp.Elapsed.TotalMilliseconds;
    Console.WriteLine($"Сумма массива из {inputArray.Count} элементов через Parallel.ForEach рассчиталась за {calcingtime} милисекунд и равна {sum}.Потоков {threadsCount}");
    taskInfos.Add(new TaskInfo { ItemsCount = inputArray.Count, CalcTimeInMs = calcingtime, CalcType = "By Task", TreadsCount = threadsCount, TotalSum = sum });
}
void CalcArrayParallelByParallel(List<int> inputArray, int threadsCount)
{
    long sum = 0;
    Stopwatch sp = new Stopwatch();
    var intsArrayList = SplitArray(inputArray.ToArray(), threadsCount);
    sp.Start();
    sum = intsArrayList.AsParallel().Select(x => CalcSum(x)).Sum();
    sp.Stop();
    double calcingtime = sp.Elapsed.TotalMilliseconds;
    Console.WriteLine($"Сумма массива из {inputArray.Count} элементов через AsParallel рассчиталась за {calcingtime} милисекунд и равна {sum}.Потоков {threadsCount}");
    taskInfos.Add(new TaskInfo { ItemsCount = inputArray.Count, CalcTimeInMs = calcingtime, CalcType = "By AsParallel", TreadsCount = threadsCount, TotalSum = sum });
}
void SaveResultsInFile()
{
    string csvString = CsvSerializer.SerializeToCsv(taskInfos);

    byte[] csvBytes = System.Text.Encoding.Unicode.GetBytes(csvString);

    File.WriteAllBytes(AppDomain.CurrentDomain.BaseDirectory + @"\Results.csv", csvBytes);
}
