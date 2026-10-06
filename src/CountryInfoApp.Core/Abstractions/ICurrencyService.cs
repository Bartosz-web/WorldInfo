using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface ICurrencyService
{
    Task<IReadOnlyList<Currency>> GetCurrenciesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default);

    Task<string> GetCurrencyNameAsync(string currencyIsoCode, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CountrySummary>> GetCountriesUsingCurrencyAsync(string currencyIsoCode, CancellationToken cancellationToken = default);
}
