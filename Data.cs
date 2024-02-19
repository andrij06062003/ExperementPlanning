namespace lr1;

public class Data
{
    private static List<string> CountryNames = new List<string> { "Ukraine", "USA", "China", "Germany", "France" };
    private static List<string> CityNames = new List<string> { "Kyiv", "New York", "Beijing", "Berlin", "Paris", "Tokyo", "Moscow", "London", "Rome", "Sydney" };


    public static List<ICountry> GenerateTestData(int dataSize)
    {
        Random random = new Random();
        List<ICountry> countries = new List<ICountry>();

        for (int i = 0; i < dataSize; i++)
        {
            ICountry country = new ICountry
            {
                Name = GetRandomCountryName(),
                City = GetRandomCityName(),
                Language = "Language" + i,
                Currency = "Currency" + i,
                Population = random.Next(1000000000), 
                Area = random.NextDouble() * 1000000,
                Description = "Description" + i,
            };
            countries.Add(country);
        }

        return countries;
    }
    
    
    
    private static string GetRandomCountryName()
    {
        Random random = new Random();
        int index = random.Next(CountryNames.Count);
        return CountryNames[index];
    }
    
    private static string GetRandomCityName()
    {
        Random random = new Random();
        int index = random.Next(CityNames.Count);
        return CityNames[index];
    }

}

