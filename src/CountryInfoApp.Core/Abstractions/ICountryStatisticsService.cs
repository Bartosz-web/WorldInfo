using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface ICountryStatisticsService
{
    /// <summary>Number of countries per continent code, largest first.</summary>
    IReadOnlyList<LabeledCount> CountByContinent(IEnumerable<CountryDetails> countries);

    /// <summary>Currency codes used by the most countries.</summary>
    IReadOnlyList<LabeledCount> TopCurrencies(IEnumerable<CountryDetails> countries, int take);

    /// <summary>Language names spoken in the most countries.</summary>
    IReadOnlyList<LabeledCount> TopLanguages(IEnumerable<CountryDetails> countries, int take);
}
