using System.Diagnostics;
namespace lr1;

static class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Введіть розмір тестових даних:");
        int dataSize = int.Parse(Console.ReadLine());
        List<ICountry> countries = Data.GenerateTestData(dataSize);
        string filePath = "data.txt";
        WriteDataToFile(filePath, countries);

       
        Stopwatch stopwatchWithoutTask = Stopwatch.StartNew();
        List<ICountry> sortedCountriesWithoutTask = Sort.SortCountries(countries);
        stopwatchWithoutTask.Stop();
        long elapsedTimeWithoutTask = stopwatchWithoutTask.ElapsedMilliseconds;
        WriteDataToFile("sortednotask.txt", sortedCountriesWithoutTask);


        Stopwatch stopwatchWithTask = Stopwatch.StartNew();
        Task<List<ICountry>> sortingTask = Task.Run(() => Sort.SortCountries(countries));
        sortingTask.Wait();
        stopwatchWithTask.Stop();
        long elapsedTimeWithTask = stopwatchWithTask.ElapsedMilliseconds;
        WriteDataToFile("sortedtask.txt", sortingTask.Result);

        Console.WriteLine("Час сортування без використання Task: " + elapsedTimeWithoutTask + " мс");
        Console.WriteLine("Час сортування з використанням Task: " + elapsedTimeWithTask + " мс");
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
