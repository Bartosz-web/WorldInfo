using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class CountryBrowserViewModel : TabViewModelBase
{
    private readonly IContinentService _continentService;
    private readonly ICountryCatalogService _catalogService;
    private Dictionary<string, IReadOnlyList<CountrySummary>> _countriesByContinent = new(StringComparer.OrdinalIgnoreCase);

    [ObservableProperty]
    private ContinentItem? _selectedContinent;

    [ObservableProperty]
    private bool _sortContinentsByCode;

    [ObservableProperty]
    private string _countryFilter = string.Empty;

    [ObservableProperty]
    private CountrySummary? _selectedCountry;

    public CountryBrowserViewModel(
        IContinentService continentService,
        ICountryCatalogService catalogService,
        CountryDetailsViewModel details)
    {
        _continentService = continentService;
        _catalogService = catalogService;
        Details = details;
    }

    public override string Title => UiText.BrowseTab;

    public CountryDetailsViewModel Details { get; }

    public ObservableCollection<ContinentItem> Continents { get; } = [];

    public ObservableCollection<CountrySummary> Countries { get; } = [];

    protected override async Task LoadAsync()
    {
        var groups = await _catalogService.GetCountriesGroupedByContinentAsync();
        _countriesByContinent = groups.ToDictionary(
            g => g.Continent.Code,
            g => (IReadOnlyList<CountrySummary>)g.Countries.OrderBy(c => c.Name, StringComparer.CurrentCulture).ToList(),
            StringComparer.OrdinalIgnoreCase);

        await LoadContinentsAsync();
    }

    private async Task LoadContinentsAsync()
    {
        var previousCode = SelectedContinent?.Code;
        var sortOrder = SortContinentsByCode ? SortOrder.ByCode : SortOrder.ByName;
        var continents = await _continentService.GetContinentsAsync(sortOrder);

        Continents.ReplaceWith(continents.Select(c => new ContinentItem(c.Code, UiText.CodeAndName(c.Code, ContinentNames.ToPolish(c.Code, c.Name)))));
        SelectedContinent = Continents.FirstOrDefault(c => c.Code == previousCode) ?? Continents.FirstOrDefault();
    }

    partial void OnSortContinentsByCodeChanged(bool value) => _ = RunAsync(LoadContinentsAsync);

    partial void OnSelectedContinentChanged(ContinentItem? value) => RefreshCountries();

    partial void OnCountryFilterChanged(string value) => RefreshCountries();

    partial void OnSelectedCountryChanged(CountrySummary? value)
    {
        if (value is not null)
        {
            _ = Details.LoadAsync(value.IsoCode);
        }
    }

    private void RefreshCountries()
    {
        var countries = SelectedContinent is not null && _countriesByContinent.TryGetValue(SelectedContinent.Code, out var list)
            ? list
            : [];

        Countries.ReplaceWith(countries.Where(c => TextFilter.Matches(CountryFilter, c.Name, c.IsoCode)));
    }
}
