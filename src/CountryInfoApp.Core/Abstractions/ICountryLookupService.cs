using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

/// <summary>
/// Single-value lookups for one country.
/// </summary>
public interface ICountryLookupService
{
    Task<string> GetCountryNameAsync(string countryIsoCode, CancellationToken cancellationToken = default);

    Task<string> GetCountryIsoCodeAsync(string countryName, CancellationToken cancellationToken = default);

    Task<string> GetCapitalCityAsync(string countryIsoCode, CancellationToken cancellationToken = default);

    Task<string> GetPhoneCodeAsync(string countryIsoCode, CancellationToken cancellationToken = default);

    Task<Currency> GetCountryCurrencyAsync(string countryIsoCode, CancellationToken cancellationToken = default);
}
