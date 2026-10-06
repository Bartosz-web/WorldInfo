using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

/// <summary>
/// Lists of countries and full country information.
/// </summary>
public interface ICountryCatalogService
{
    Task<IReadOnlyList<CountrySummary>> GetCountriesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ContinentCountries>> GetCountriesGroupedByContinentAsync(CancellationToken cancellationToken = default);

    Task<CountryDetails> GetCountryDetailsAsync(string countryIsoCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CountryDetails>> GetAllCountryDetailsAsync(CancellationToken cancellationToken = default);
}
