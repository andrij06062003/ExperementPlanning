namespace lr1;

public class Sort
{
    public static List<ICountry> SortCountries(List<ICountry> countries)
    {
        
        countries.Sort((c1, c2) => c1.Name.CompareTo(c2.Name));
        
        countries.Sort((c1, c2) => c1.Capital.CompareTo(c2.Capital));
        
        countries.Sort((c1, c2) => c1.Language.CompareTo(c2.Language));

        return countries;
    }
}
