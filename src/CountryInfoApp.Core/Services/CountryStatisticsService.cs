using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Services;

public sealed class CountryStatisticsService : ICountryStatisticsService
{
    public IReadOnlyList<LabeledCount> CountByContinent(IEnumerable<CountryDetails> countries)
    {
        ArgumentNullException.ThrowIfNull(countries);
        return CountBy(countries.Select(c => c.ContinentCode));
    }

    public IReadOnlyList<LabeledCount> TopCurrencies(IEnumerable<CountryDetails> countries, int take)
    {
        ArgumentNullException.ThrowIfNull(countries);
        return CountBy(countries.Select(c => c.CurrencyIsoCode)).Take(take).ToList();
    }

    public IReadOnlyList<LabeledCount> TopLanguages(IEnumerable<CountryDetails> countries, int take)
    {
        ArgumentNullException.ThrowIfNull(countries);
        return CountBy(countries.SelectMany(c => c.Languages).Select(l => l.Name)).Take(take).ToList();
    }

    private static List<LabeledCount> CountBy(IEnumerable<string> keys) =>
        keys
            .Where(key => !string.IsNullOrWhiteSpace(key))
            .GroupBy(key => key, StringComparer.OrdinalIgnoreCase)
            .Select(group => new LabeledCount(group.Key, group.Count()))
            .OrderByDescending(item => item.Count)
            .ThenBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();
}
