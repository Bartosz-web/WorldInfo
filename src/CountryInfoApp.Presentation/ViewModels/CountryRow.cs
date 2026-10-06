using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

/// <summary>
/// A country formatted for the "all countries" table.
/// </summary>
public sealed record CountryRow(CountryDetails Country)
{
    public string IsoCode => Country.IsoCode;

    public string Name => Country.Name;

    public string CapitalCity => Country.CapitalCity;

    public string Continent => ContinentNames.ToPolish(Country.ContinentCode);

    public string PhoneCode => UiText.PhoneCode(Country.PhoneCode);

    public string Currency => Country.CurrencyIsoCode;

    public string Languages => string.Join(", ", Country.Languages.Select(l => l.Name));
}
