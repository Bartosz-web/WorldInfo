namespace CountryInfoApp.Infrastructure.Soap;

/// <summary>
/// Operation and parameter names exactly as declared in the service WSDL.
/// </summary>
internal static class CountryInfoOperations
{
    public const string ListOfContinentsByName = nameof(ListOfContinentsByName);
    public const string ListOfContinentsByCode = nameof(ListOfContinentsByCode);
    public const string ListOfCountryNamesByName = nameof(ListOfCountryNamesByName);
    public const string ListOfCountryNamesByCode = nameof(ListOfCountryNamesByCode);
    public const string ListOfCountryNamesGroupedByContinent = nameof(ListOfCountryNamesGroupedByContinent);
    public const string FullCountryInfo = nameof(FullCountryInfo);
    public const string FullCountryInfoAllCountries = nameof(FullCountryInfoAllCountries);
    public const string CountryName = nameof(CountryName);
    public const string CountryISOCode = nameof(CountryISOCode);
    public const string CapitalCity = nameof(CapitalCity);
    public const string CountryIntPhoneCode = nameof(CountryIntPhoneCode);
    public const string CountryCurrency = nameof(CountryCurrency);
    public const string ListOfCurrenciesByName = nameof(ListOfCurrenciesByName);
    public const string ListOfCurrenciesByCode = nameof(ListOfCurrenciesByCode);
    public const string CurrencyName = nameof(CurrencyName);
    public const string CountriesUsingCurrency = nameof(CountriesUsingCurrency);
    public const string ListOfLanguagesByName = nameof(ListOfLanguagesByName);
    public const string ListOfLanguagesByCode = nameof(ListOfLanguagesByCode);
    public const string LanguageName = nameof(LanguageName);
    public const string LanguageISOCode = nameof(LanguageISOCode);

    public static class Parameters
    {
        public const string CountryIsoCode = "sCountryISOCode";
        public const string CountryName = "sCountryName";
        public const string CurrencyIsoCode = "sCurrencyISOCode";
        public const string IsoCurrencyCode = "sISOCurrencyCode";
        public const string IsoCode = "sISOCode";
        public const string Language = "sLanguage";
    }

    public static readonly IReadOnlyDictionary<string, string> NoParameters = new Dictionary<string, string>();

    public static IReadOnlyDictionary<string, string> With(string name, string value) =>
        new Dictionary<string, string> { [name] = value.Trim() };
}
