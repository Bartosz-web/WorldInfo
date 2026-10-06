namespace CountryInfoApp.Core.Models;

public sealed record CountryDetails(
    string IsoCode,
    string Name,
    string CapitalCity,
    string PhoneCode,
    string ContinentCode,
    string CurrencyIsoCode,
    string FlagUrl,
    IReadOnlyList<Language> Languages);
