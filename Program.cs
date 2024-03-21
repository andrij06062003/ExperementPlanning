using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lr2;

namespace Lr4
{
    static class Program
    {
        private static readonly object _lockObj = new object();
        private static readonly Mutex _mutex = new Mutex();
        private static int _counter = 0;

        public static void Main(string[] args)
        {
            

            Console.WriteLine("Введіть розмір тестових даних:");
            int dataSize = int.Parse(Console.ReadLine());
            List<ICountry> countries = Data.GenerateTestData(dataSize);
            string filePath = "data.txt";
            WriteDataToFile(filePath, countries);

            Task<List<ICountry>> parentTask = Task.Factory.StartNew(() =>
            {
                Stopwatch parentStopwatch = Stopwatch.StartNew();
                List<ICountry> sortedCountries = Sort.SortCountries(countries);
                WriteDataToFile("sorted_parent.txt", sortedCountries);
                parentStopwatch.Stop();
                Console.WriteLine("Сортування parentTask. Час виконання: " + parentStopwatch.ElapsedMilliseconds + " мс");
                return sortedCountries;
            });

            Task childTask1 = parentTask.ContinueWith((prevTask) =>
            {
                Stopwatch childStopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionLock(prevTask.Result);
                childStopwatch.Stop();
                Console.WriteLine("Вкладений потік Lock. Час виконання: " + childStopwatch.ElapsedMilliseconds + " мс");
            });

            Task childTask2 = parentTask.ContinueWith((prevTask) =>
            {
                Stopwatch childStopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionMonitor(prevTask.Result);
                childStopwatch.Stop();
                Console.WriteLine("Вкладений потік Monitor. Час виконання:  " + childStopwatch.ElapsedMilliseconds + " мс");
            });

            Task childTask3 = parentTask.ContinueWith((prevTask) =>
            {
                Stopwatch childStopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionMutex(prevTask.Result);
                childStopwatch.Stop();
                Console.WriteLine("Вкладений потік Mutex. Час виконання:  " + childStopwatch.ElapsedMilliseconds + " мс");
            });

            Stopwatch continueWhenAllStopwatch = Stopwatch.StartNew();
            Task continueWhenAllTask = Task.Factory.ContinueWhenAll(new[] { childTask1, childTask2, childTask3 }, (tasks) =>
            {
                continueWhenAllStopwatch.Stop();
                Console.WriteLine("ContinueWhenAll час виконання: " + continueWhenAllStopwatch.ElapsedMilliseconds + " мс");
            });

            Stopwatch continueWhenAnyStopwatch = Stopwatch.StartNew();
            Task continueWhenAnyTask = Task.Factory.ContinueWhenAny(new[] { childTask1, childTask2, childTask3 }, (completedTask) =>
            {
                continueWhenAnyStopwatch.Stop();
                Console.WriteLine("ContinueWhenAny час виконання: " + continueWhenAnyStopwatch.ElapsedMilliseconds + " мс");
            });

            Task.WaitAll(continueWhenAllTask, continueWhenAnyTask);

            
           
        }

        private static void FunctionWithCriticalSectionLock(List<ICountry> countries)
        {
            lock (_lockObj)
            {
                List<ICountry> sortedCountries = Sort.SortCountries(countries);
                WriteDataToFile("sorted_lock.txt", sortedCountries);
            }
        }

        private static void FunctionWithCriticalSectionMonitor(List<ICountry> countries)
        {
            Monitor.Enter(_lockObj);
            try
            {
                List<ICountry> sortedCountries = Sort.SortCountries(countries);
                WriteDataToFile("sorted_monitor.txt", sortedCountries);
            }
            finally
            {
                Monitor.Exit(_lockObj);
            }
        }

        private static void FunctionWithCriticalSectionMutex(List<ICountry> countries)
        {
            _mutex.WaitOne();
            try
            {
                List<ICountry> sortedCountries = Sort.SortCountries(countries);
                WriteDataToFile("sorted_mutex.txt", sortedCountries);
            }
            finally
            {
                _mutex.ReleaseMutex();
            }
        }

        private static void WriteDataToFile(string filePath, List<ICountry> countries)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (ICountry country in countries)
                {
                    writer.WriteLine($"{country.Name},{country.City},{country.Language},{country.Currency},{country.Population},{country.Area},{country.Description}");
                }
            }
        }
    }
}
