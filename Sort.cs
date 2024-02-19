namespace lr1;

public class Sort
{
    public static List<ICountry> SortCountries(List<ICountry> countries)
    {
        
        var sortedCountries = countries.OrderBy(c => c.Name)
            .ThenBy(c => c.City)
            .ThenBy(c => c.Population)
            .ToList();
        return sortedCountries;
    }
}
