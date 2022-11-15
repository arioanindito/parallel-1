using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Diagnostics;

namespace checkBarcode;
class Program
{
    public static int element = 100_000;
    static readonly object locker = new object();
    public static int[] nthreads = {1, 2, 3, 4, 6};
    public static IDictionary<int, int> SearchTypeIndex = new Dictionary<int, int>()
    {
        {1,30},
        {7,15},
        {10,8} 
    };

    static void Main(string[] args)
    {
        var stopwatch = new Stopwatch();

        Console.WriteLine("Question 2");

        //generating list of barcodes
        var list = GenerateBarcodes();

        foreach (var n in nthreads)
        {
            stopwatch.Reset();
            stopwatch.Start();
            Question02(n, list);
            stopwatch.Stop();
            Console.Write($"It takes {stopwatch.ElapsedMilliseconds} miliseconds to process {n} threads\n");
        }

        Console.WriteLine($"Completed");
        Console.Read();
    }
        
    static void Question02(int numberOfthreads, List<string> list)
    {
        var typeOfelement = ThreadBarcodeSearch(numberOfthreads,list);
    }

    static IDictionary<int, List<string>> ThreadBarcodeSearch(int numberOfthreads, List<string> list)
    {
        //making sublists for threads searching
        var subLists = GenerateSubLists(list, numberOfthreads);

        IDictionary<int, List<string>> typeOfelement = new Dictionary<int, List<string>>();
        var newlist = new List<string>();
        foreach (KeyValuePair<int, int> value in SearchTypeIndex)
        {
            typeOfelement.Add(value.Key, newlist);
        }

        //searching barcodes with threads
        ThreadsWorkBarcodes(numberOfthreads, subLists, typeOfelement);

        return typeOfelement;
    } 

    static List<string> GenerateBarcodes()
    {
        var length = element;
        var array = new string[length];

        //add Searched elements to array to make sure
        //there are at least needed element of them
        var list = new List<string>();
        foreach (KeyValuePair<int, int> value in SearchTypeIndex)
        {
            for (var i = 0; i < value.Value; i++)
            {
                Random random = new Random();
                var barcode = $"{random.Next(0, 99999).ToString("00000")}{value.Key.ToString("000")}";
                list.Add(barcode);
            }
        }
        for (var y = 0; y < list.Count; y++)
        {
            array[y] = list[y];
        }

        //shuffle generated barcodes and fulfill remain cells in array
        for (var i = 0; i < length; i++)
        {
            // var temp = "";
            Random random = new Random();
            var y = i + random.Next(0, length - i);
            // temp = array[y];
            // array[y] = array[i];
            // array[i] = temp;

            if (array[i] == null)
            {
                array[i] = $"{random.Next(0, 99999).ToString("00000")}{random.Next(1, 100).ToString("000")}"; ;
            }
        }
        return array.ToList();
    }

    static List<string>[] GenerateSubLists(List<string> origList, int numberOfthreads)
    {
        var list = new List<string>(origList);
        List<string>[] sublists = new List<string>[numberOfthreads];
        var splitFactor = Math.Ceiling(Convert.ToDouble(list.Count) / Convert.ToDouble(numberOfthreads));
        for (int i = 0; i < numberOfthreads; i++)
        {
            sublists[i] = new List<string>();
        }

        //add elements until the length of sublist is equal to splitFactor
        for (int i = 0; i < list.Count; i++)
        {
            int j = Convert.ToInt32(Math.Floor(Convert.ToDouble(i) / Convert.ToDouble(splitFactor)));
            var value = list[i];
            sublists[j].Add(value);
        }
        return sublists;
    }

    static void ThreadsWorkBarcodes(int numberOfthreads, List<string>[] subLists, IDictionary<int, List<string>> Typeselements)
    {
        //common buffer for all threads
        List<string>[] elements = new List<string>[SearchTypeIndex.Count];
        for (var i = 0; i < SearchTypeIndex.Count; i++)
        {
            elements[i] = new List<string>();
        }

        //start threads
        var threads = new List<Thread>();
        for (int i = 0; i < numberOfthreads; i++)
        {
            var list = subLists[i];
            var thread = new Thread(() => FindBarcodes(list, Typeselements, elements));
            thread.Start();
            threads.Add(thread);
        }

        //stop threads
        foreach (var thread in threads)
        {
            thread.Join();
        }
    }

    static IDictionary<int, List<string>> FindBarcodes(List<string> list, IDictionary<int, List<string>> typeOfelement, List<string>[] elements)
    {
        var j = 0;
        foreach (KeyValuePair<int, int> value in SearchTypeIndex)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var type = value.Key.ToString("000");
                var barcode = list[i].Substring(5, 3);

                if (elements[j].Count < value.Value)
                {
                    if (barcode == type)
                    {
                        elements[j].Add(list[i]);
                    }                        
                }
                
                else
                {
                    break;
                }
            }
            typeOfelement[value.Key] = elements[j];
            j++;
        }
        return typeOfelement;

    }
}
