using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace lr1
{
    static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Введіть розмір тестових даних:");
            int dataSize = int.Parse(Console.ReadLine());
            List<ICountry> countries = Data.GenerateTestData(dataSize);
            string filePath = "data.txt";
            WriteDataToFile(filePath, countries);

            Stopwatch stopwatchWithoutThread = Stopwatch.StartNew();
            List<ICountry> sortedCountriesWithoutThread = Sort.SortCountries(countries);
            stopwatchWithoutThread.Stop();
            long elapsedTimeWithoutThread = stopwatchWithoutThread.ElapsedMilliseconds;
            WriteDataToFile("sortednotask.txt", sortedCountriesWithoutThread);

            Stopwatch stopwatchWithThreads = Stopwatch.StartNew();
            List<ICountry> sortedCountries1 = null;
            List<ICountry> sortedCountries2 = null;

            Thread thread1 = new Thread(() =>
            {
                sortedCountries1 = Sort.SortCountries(countries.GetRange(0, dataSize / 2));
            });

            Thread thread2 = new Thread(() =>
            {
                sortedCountries2 = Sort.SortCountries(countries.GetRange(dataSize / 2, dataSize - dataSize / 2));
            });

            thread1.Start();
            thread2.Start();
            thread1.Join();
            thread2.Join();

            List<ICountry> mergedSortedCountries = Merge(sortedCountries1, sortedCountries2);
            stopwatchWithThreads.Stop();
            long elapsedTimeWithThreads = stopwatchWithThreads.ElapsedMilliseconds;

            WriteDataToFile("sortedwiththreads.txt", mergedSortedCountries);

            Console.WriteLine("Час сортування без використання Thread: " + elapsedTimeWithoutThread + " мс");
            Console.WriteLine("Час сортування з використанням Thread: " + elapsedTimeWithThreads + " мс");
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

        private static List<ICountry> Merge(List<ICountry> list1, List<ICountry> list2)
        {
            List<ICountry> mergedList = new List<ICountry>();
            mergedList.AddRange(list1);
            mergedList.AddRange(list2);
            return mergedList;
        }
    }
}