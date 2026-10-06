using System.Xml.Linq;
using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Infrastructure.Soap;

/// <summary>
/// Translates the service's XML result elements into domain models.
/// </summary>
public sealed class CountryInfoXmlMapper
{
    private readonly XNamespace _ns;

    public CountryInfoXmlMapper(SoapClientOptions options)
    {
        _ns = options.ServiceNamespace;
    }

    public IReadOnlyList<Continent> ToContinents(XElement result) =>
        result.Elements(_ns + "tContinent").Select(ToContinent).ToList();

    public IReadOnlyList<CountrySummary> ToCountrySummaries(XElement result) =>
        result.Elements(_ns + "tCountryCodeAndName").Select(ToCountrySummary).ToList();

    public IReadOnlyList<ContinentCountries> ToContinentCountries(XElement result) =>
        result.Elements(_ns + "tCountryCodeAndNameGroupedByContinent")
            .Select(group => new ContinentCountries(
                ToContinent(group.Element(_ns + "Continent") ?? new XElement(_ns + "Continent")),
                ToCountrySummaries(group.Element(_ns + "CountryCodeAndNames") ?? new XElement(_ns + "CountryCodeAndNames"))))
            .ToList();

    public IReadOnlyList<Currency> ToCurrencies(XElement result) =>
        result.Elements(_ns + "tCurrency").Select(ToCurrency).ToList();

    public Currency ToCurrency(XElement element)
    {
        var currency = new Currency(Text(element, "sISOCode"), Text(element, "sName"));
        EnsureFound(currency.Name);
        return currency;
    }

    public IReadOnlyList<Language> ToLanguages(XElement result) =>
        result.Elements(_ns + "tLanguage").Select(ToLanguage).ToList();

    public CountryDetails ToCountryDetails(XElement element)
    {
        var details = new CountryDetails(
            IsoCode: Text(element, "sISOCode"),
            Name: Text(element, "sName"),
            CapitalCity: Text(element, "sCapitalCity"),
            PhoneCode: Text(element, "sPhoneCode"),
            ContinentCode: Text(element, "sContinentCode"),
            CurrencyIsoCode: Text(element, "sCurrencyISOCode"),
            FlagUrl: Text(element, "sCountryFlag"),
            Languages: ToLanguages(element.Element(_ns + "Languages") ?? new XElement(_ns + "Languages")));
        EnsureFound(details.Name);
        return details;
    }

    public IReadOnlyList<CountryDetails> ToCountryDetailsList(XElement result) =>
        result.Elements(_ns + "tCountryInfo").Select(ToCountryDetails).ToList();

    /// <summary>
    /// Reads a single string result. The service signals a missing entity with a text message.
    /// </summary>
    public static string ToText(XElement result)
    {
        var value = result.Value.Trim();
        EnsureFound(value);
        return value;
    }

    private static void EnsureFound(string value)
    {
        if (string.IsNullOrWhiteSpace(value)
            || value.Contains("not found", StringComparison.OrdinalIgnoreCase)
            || value.Contains("No country found", StringComparison.OrdinalIgnoreCase))
        {
            throw new EntityNotFoundException($"The service returned no data ('{value}').");
        }
    }

    private Continent ToContinent(XElement element) => new(Text(element, "sCode"), Text(element, "sName"));

    private CountrySummary ToCountrySummary(XElement element) => new(Text(element, "sISOCode"), Text(element, "sName"));

    private Language ToLanguage(XElement element) => new(Text(element, "sISOCode"), Text(element, "sName"));

    private string Text(XElement element, string name) => element.Element(_ns + name)?.Value.Trim() ?? string.Empty;
}
