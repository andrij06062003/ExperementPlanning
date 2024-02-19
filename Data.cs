namespace lr1;

public class Data
{
    public static List<ICountry> GenerateTestData(int dataSize)
    {
        Random random = new Random();
        List<ICountry> countries = new List<ICountry>();

        for (int i = 0; i < dataSize; i++)
        {
            ICountry country = new ICountry
            {
                Name = "Country" + i,
                Capital = "Capital" + i,
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
}