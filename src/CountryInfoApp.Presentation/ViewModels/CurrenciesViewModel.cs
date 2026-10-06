using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class CurrenciesViewModel : TabViewModelBase
{
    private readonly ICurrencyService _currencyService;
    private IReadOnlyList<Currency> _allCurrencies = [];

    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private bool _sortByCode;

    [ObservableProperty]
    private Currency? _selectedCurrency;

    public CurrenciesViewModel(ICurrencyService currencyService)
    {
        _currencyService = currencyService;
    }

    public override string Title => UiText.CurrenciesTab;

    public ObservableCollection<Currency> Currencies { get; } = [];

    public ObservableCollection<CountrySummary> CountriesUsingCurrency { get; } = [];

    protected override async Task LoadAsync()
    {
        _allCurrencies = await _currencyService.GetCurrenciesAsync(SortByCode ? SortOrder.ByCode : SortOrder.ByName);
        ApplyFilter();
    }

    partial void OnFilterChanged(string value) => ApplyFilter();

    partial void OnSortByCodeChanged(bool value) => _ = RunAsync(LoadAsync);

    partial void OnSelectedCurrencyChanged(Currency? value)
    {
        CountriesUsingCurrency.Clear();
        if (value is not null)
        {
            _ = RunAsync(async () =>
                CountriesUsingCurrency.ReplaceWith(await _currencyService.GetCountriesUsingCurrencyAsync(value.IsoCode)));
        }
    }

    private void ApplyFilter() =>
        Currencies.ReplaceWith(_allCurrencies.Where(c => TextFilter.Matches(Filter, c.IsoCode, c.Name)));
}
