using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Lr2;

namespace Lr3
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

         
            Task task1 = new Task(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionLock(countries);
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Час сортування захищеного lock: " + elapsedTime + " мс");
            });
            task1.Start();

            
            Task task2 = Task.Factory.StartNew(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionMonitor(countries);
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Час сортування захищеного Monitor.Enter: " + elapsedTime + " мс");
            });

           
            Task task3 = Task.Factory.StartNew(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionMutex(countries);
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Час сортування захищеного Mutex: " + elapsedTime + " мс");
            });

         
            Task task4 = Task.Factory.StartNew(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                FunctionWithCriticalSectionInterlocked();
                stopwatch.Stop();
                long elapsedTime = stopwatch.ElapsedMilliseconds;
                Console.WriteLine("Час сортування захищеного Interlocked: " + elapsedTime + " мс");
            });

            Task.WaitAll(task1, task2, task3, task4);
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

        private static void FunctionWithCriticalSectionInterlocked()
        {
            int count = 0;
            using (StreamReader reader = new StreamReader("data.txt"))
            {
                while (reader.ReadLine() != null)
                {
                    count++;
                }
            }
            Interlocked.Add(ref _counter, count);
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
