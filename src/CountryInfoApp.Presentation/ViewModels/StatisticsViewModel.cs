using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class StatisticsViewModel : TabViewModelBase
{
    private const int TopCount = 10;

    private readonly ICountryCatalogService _catalogService;
    private readonly ICountryStatisticsService _statisticsService;

    [ObservableProperty]
    private string _summary = string.Empty;

    public StatisticsViewModel(ICountryCatalogService catalogService, ICountryStatisticsService statisticsService)
    {
        _catalogService = catalogService;
        _statisticsService = statisticsService;
    }

    public override string Title => UiText.StatisticsTab;

    public ObservableCollection<BarItem> CountriesPerContinent { get; } = [];

    public ObservableCollection<BarItem> TopCurrencies { get; } = [];

    public ObservableCollection<BarItem> TopLanguages { get; } = [];

    protected override async Task LoadAsync()
    {
        var countries = await _catalogService.GetAllCountryDetailsAsync();

        Summary = UiText.CountriesTotal(countries.Count);
        CountriesPerContinent.ReplaceWith(ToBars(_statisticsService.CountByContinent(countries), code => ContinentNames.ToPolish(code)));
        TopCurrencies.ReplaceWith(ToBars(_statisticsService.TopCurrencies(countries, TopCount), code => code));
        TopLanguages.ReplaceWith(ToBars(_statisticsService.TopLanguages(countries, TopCount), name => name));
    }

    private static IEnumerable<BarItem> ToBars(IReadOnlyList<LabeledCount> counts, Func<string, string> label)
    {
        var maximum = counts.Count == 0 ? 0 : counts.Max(c => c.Count);
        return counts.Select(c => new BarItem(label(c.Key), c.Count, maximum));
    }
}
