using System.Diagnostics;
using System.Threading;

namespace task1
{
  class Program
  {
    public static int[] nThreads = { 1, 2, 3, 4, 6 };

    static readonly object locker = new object();

    static void Main(string[] args)
    {
      var stopwatch = new Stopwatch();

      // Generate a random array of 100,000 elements with a value of min 0 and max 100
      int element = 100000;
      int Min = 0;
      int Max = 100;
      Random randNum = new Random();
      int[] array = Enumerable
          .Repeat(0, element)
          .Select(i => randNum.Next(Min, Max))
          .ToArray();

      foreach (var n in nThreads)
      {
        stopwatch.Reset();
        stopwatch.Start();
        parallelSorting(array.ToList(), n);
        stopwatch.Stop();
        Console.Write($"{stopwatch.ElapsedMilliseconds} miliseconds to processed {n} threads\n");
      }

      Console.WriteLine();
      Console.WriteLine($"All Process are Completed");
      Console.Read();
    }

    static int[] parallelSorting(List<int> array, int numberOfthreads)
    {
      // making sublists
      var maxNumber = array.Max();
      List<int>[] subLists = new List<int>[numberOfthreads];
      var split = maxNumber / numberOfthreads;

      for (var j = 0; j < numberOfthreads - 1; j++)
      {
        subLists[j] = new List<int>();

        for (int i = 0; i < array.Count; i++)
        {
          //Choosing values less then split factor for current sublist
          if (array[i] <= split * (j + 1))
          {
            var value = array[i];
            subLists[j].Add(value);
          }
        }
        //To avoid dublicates remove current value from array                       
        array.RemoveAll(a => subLists[j].Contains(a));
      }
      //add remaining values to the last sublist
      subLists[subLists.Length - 1] = array;

      // starting threads
      var threads = new List<Thread>();

      for (int i = 0; i < numberOfthreads; i++)
      {
        var list = subLists[i];
        var thread = new Thread(() => bubbleSort(list));
        thread.Start();
        threads.Add(thread);
      }

      //stopping threads
      foreach (var thread in threads)
      {
        thread.Join();
      }

      //join all sublists to sorted list
      var sortedList = new List<int>();
      foreach (var subList in subLists)
      {
        foreach (var list in subList)
        {
          sortedList.Add(list);
        }
      }

      return sortedList.ToArray();
    }

    static void bubbleSort(List<int> array)
    {
      int counter;

      for (var y = 0; y < array.Count - 1; y++)
      {
        for (var i = 0; i < array.Count - 1; i++)
        {
          if (array[i] > array[i + 1])
          {
            counter = array[i + 1];
            array[i + 1] = array[i];
            array[i] = counter;
          }
        }
      }
    }
  }
}