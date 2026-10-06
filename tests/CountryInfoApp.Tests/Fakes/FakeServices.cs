using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Tests.Fakes;

internal sealed class FakeContinentService : IContinentService
{
    public IReadOnlyList<Continent> Continents { get; set; } =
        [new("AF", "Africa"), new("EU", "Europe")];

    public Task<IReadOnlyList<Continent>> GetContinentsAsync(SortOrder sortOrder, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Continent>>(
            sortOrder == SortOrder.ByCode
                ? Continents.OrderBy(c => c.Code, StringComparer.Ordinal).ToList()
                : Continents.OrderBy(c => c.Name, StringComparer.Ordinal).ToList());
}

internal sealed class FakeCountryCatalogService : ICountryCatalogService
{
    public IReadOnlyList<CountryDetails> Countries { get; set; } = [];

    public bool ShouldFail { get; set; }

    public Task<IReadOnlyList<CountrySummary>> GetCountriesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default) =>
        Run(() => (IReadOnlyList<CountrySummary>)Countries.Select(c => new CountrySummary(c.IsoCode, c.Name)).ToList());

    public Task<IReadOnlyList<ContinentCountries>> GetCountriesGroupedByContinentAsync(CancellationToken cancellationToken = default) =>
        Run(() => (IReadOnlyList<ContinentCountries>)Countries
            .GroupBy(c => c.ContinentCode)
            .Select(g => new ContinentCountries(new Continent(g.Key, g.Key), g.Select(c => new CountrySummary(c.IsoCode, c.Name)).ToList()))
            .ToList());

    public Task<CountryDetails> GetCountryDetailsAsync(string countryIsoCode, CancellationToken cancellationToken = default) =>
        Run(() => Countries.FirstOrDefault(c => c.IsoCode == countryIsoCode) ?? throw new EntityNotFoundException());

    public Task<IReadOnlyList<CountryDetails>> GetAllCountryDetailsAsync(CancellationToken cancellationToken = default) =>
        Run(() => Countries);

    private Task<T> Run<T>(Func<T> result) =>
        ShouldFail ? Task.FromException<T>(new CountryInfoServiceException("down")) : Task.FromResult(result());
}

internal sealed class FakeCurrencyService : ICurrencyService
{
    public Task<IReadOnlyList<Currency>> GetCurrenciesAsync(SortOrder sortOrder, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Currency>>([new("EUR", "Euro"), new("PLN", "Zloty")]);

    public Task<string> GetCurrencyNameAsync(string currencyIsoCode, CancellationToken cancellationToken = default) =>
        currencyIsoCode == "EUR" ? Task.FromResult("Euro") : Task.FromException<string>(new EntityNotFoundException());

    public Task<IReadOnlyList<CountrySummary>> GetCountriesUsingCurrencyAsync(string currencyIsoCode, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<CountrySummary>>([new("DE", "Germany"), new("FR", "France")]);
}
