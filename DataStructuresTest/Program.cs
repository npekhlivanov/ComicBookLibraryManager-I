using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using static DataStructuresTest.EnumerableCompositor;

namespace DataStructuresTest
{
    class Program
    {
        class IntComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                return x - y;
            }
        }

        static void Main(string[] args)
        {
            var list1 = new List<int> { 1, 2, 3, 4, 5 };
            var list2 = new List<int> { 2, 4, 6, 8, 10 };
            var set1 = new HashSet<int> { 3, 6, 9, 12, 15 };
            var array1 = new[] { 4, 8, 12, 16, 20 };
            //var ec = new EnumerableCompositor<int>(new IEnumerable<int>[] { list1, list2, set1, array1 });
            //var ec = new EnumerableCompositor<int> { list1, list2, set1, array1 }; // adding a default constructor and an Add() method allows this form of initialization

            //var ec = EnumerableCompositor.EC(list1, list2, set1, array1); 
            // adding the "params" keyword for the last parameter allows any number of args to be passed; no need to specify the type either

            //foreach (var item in ec)
            // adding the using static directive allows to skip the class name "EnumerableCompositor" and use EC() in the foreach() loop
            foreach (var item in EC(list1, list2, set1, array1))
            {
                if (item % 2 == 0)
                {
                    Console.Write(item.ToString() + ',');
                }
            }

            int numOdd = EC(list1, list2, set1, array1).Count(x => x % 2 != 0);

            var firstThree = Utils.Take(list1, 3); // no need to specify the type, as it is inferred by the compiler

            var set = EC(list1, list2, set1, array1).To<HashSet<int>>();

            // Sorting a list
            var bigList = EC(list1, list2, set1, array1).ToList();
            var bigList2 = new List<int>(bigList);
            bigList.Sort(new IntComparer()); // use IComparer

            Comparison<int> intComparer = (x, y) => x.CompareTo(y); // declare Comparison using lambda
            bigList2.Sort(intComparer);
            int compareInts(int x, int y) { return x - y; }; // delare a local function
            bigList2.Sort(compareInts);

            // The next 2 variants below produce the same code
            bigList2.Sort(
                comparison: delegate (int x, int y) // declare Comparison delegate inline
                {
                    return x - y;
                });
            bigList2.Sort((x, y) => x.CompareTo(y)); // use lambda

            TestCollections();

        }

        static void TestArrays()
        {
            // jagged array
            int maxFactor = 4;
            int[][] res = new int[maxFactor + 1][];
            for (int i = 0; i <= maxFactor; i++)
            {
                var row = new int[maxFactor + 1];
                for (int j = 0; j <= maxFactor; j++)
                {
                    row[j] = i * j;
                }
                res[i] = row;
            }

            // multidimensional array (2D)
            int[,] array = new int[maxFactor, maxFactor + 1];
            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    array[i, j] = i * j;
                }
            }
        }

        static void TestCollections()
        {
            Console.WriteLine();
            int numEnries = 50000;
            int numLocates = 10000;

            var list = new List<string>();
            var set = new HashSet<string>();
            //var dict = new Dictionary<int, string>();
            var dict = new Dictionary<string, int>();
            var random = new Random(10000);
            var stopwatch1 = new Stopwatch();
            var stopwatch2 = new Stopwatch();
            var stopwatch3 = new Stopwatch();

            for (int i = 0; i < numEnries; i++)
            {
                var value = random.Next().ToString();
                stopwatch1.Start();
                list.Add(value);
                stopwatch1.Stop();

                stopwatch2.Start();
                set.Add(value);
                stopwatch2.Stop();

                stopwatch3.Start();
                //dict.Add(i, value);
                dict.Add(value, i);
                stopwatch3.Stop();
            }
            Console.WriteLine(string.Format("Time to add {0} elements to list: {1} ms", numEnries, stopwatch1.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to add {0} elements to set: {1} ms", numEnries, stopwatch2.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to add {0} elements to dictionary: {1} ms", numEnries, stopwatch3.ElapsedMilliseconds));

            stopwatch1.Reset();
            stopwatch2.Reset();
            stopwatch3.Reset();
            var stopwatch4 = new Stopwatch();
            for (int i = 0; i < numLocates; i++)
            {
                int index = random.Next(100, numEnries);
                stopwatch4.Start();
                var value = list[index];
                stopwatch4.Stop();

                stopwatch1.Start();
                var valueIndex = list.IndexOf(value);
                stopwatch1.Stop();
                if (valueIndex != index)
                {
                    Console.WriteLine(string.Format("Mismatched index for element {0}", index));
                }

                stopwatch2.Start();
                var valueFound = set.Contains(value);
                stopwatch2.Stop();
                if (!valueFound)
                {
                    Console.WriteLine(string.Format("Value not found in set: {0}", value));
                }

                stopwatch3.Start();
                //var found = dict.TryGetValue(index, out value);
                var found = dict.TryGetValue(value, out index);
                stopwatch3.Stop();
                if (!found)
                {
                    Console.WriteLine(string.Format("Value not found in dictionary: {0}", value));
                }
            }
            Console.WriteLine(string.Format("Time to get {0} elements in the list by index: {1} ms", numLocates, stopwatch4.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to locate {0} elements in the list: {1} ms", numLocates, stopwatch1.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to locate {0} elements in the set: {1} ms", numLocates, stopwatch2.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to locate {0} elements in the dictionary: {1} ms", numLocates, stopwatch3.ElapsedMilliseconds));

            stopwatch1.Reset();
            stopwatch2.Reset();
            stopwatch3.Reset();
            var listCopy = list.ToList();
            var list2 = list.ToList();
            for (int i = 0; i < numEnries; i++)
            {
                var value = listCopy[i];

                int index = random.Next(0, list.Count - 1);
                stopwatch1.Start();
                list.Remove(value);
                stopwatch1.Stop();

                stopwatch4.Start();
                list2.RemoveAt(index);
                stopwatch4.Stop();

                stopwatch2.Start();
                set.Remove(value);
                stopwatch2.Stop();

                stopwatch3.Start();
                //dict.Remove(index);
                dict.Remove(value);
                stopwatch3.Stop();
            }
            Console.WriteLine(string.Format("Time to remove {0} elements from the list by value: {1} ms", numLocates, stopwatch1.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to remove {0} elements from the list by index: {1} ms", numLocates, stopwatch4.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to remove {0} elements from the set: {1} ms", numLocates, stopwatch2.ElapsedMilliseconds));
            Console.WriteLine(string.Format("Time to remove {0} elements from the dictionary: {1} ms", numLocates, stopwatch3.ElapsedMilliseconds));

            // results:
            //Time to add 10000 elements to list: 1 ms
            //Time to add 10000 elements to set: 1 ms
            //Time to locate 1000 elements in the list: 65 ms
            //Time to locate 1000 elements in the set: 0 ms
            //Time to remove 1000 elements in the list by value: 23 ms
            //Time to remove 1000 elements in the list by position: 13 ms
            //Time to remove 1000 elements in the set: 2 ms


            //Time to add 50000 elements to list: 6 ms
            //Time to add 50000 elements to set: 17 ms
            //Time to add 50000 elements to dictionary: 8 ms (int key)
            //Time to add 50000 elements to dictionary: 12 ms (string key)

            //Time to get 10000 elements in the list by index: 1 ms
            //Time to locate 10000 elements in the list: 3169 ms
            //Time to locate 10000 elements in the set: 5 ms
            //Time to locate 10000 elements in the dictionary: 4 ms

            //Time to remove 10000 elements from the list by value: 816 ms
            //Time to remove 10000 elements from the list by position: 414 ms
            //Time to remove 10000 elements from the set: 15 ms
            //Time to remove 10000 elements from the dictionary: 12 ms
        }
    }
}

