using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Presentation.Abstractions;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class AllCountriesViewModel : TabViewModelBase
{
    private readonly ICountryCatalogService _catalogService;
    private readonly ICountryExporter _exporter;
    private readonly IFileDialogService _fileDialog;
    private IReadOnlyList<CountryRow> _allRows = [];

    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private string? _statusMessage;

    public AllCountriesViewModel(ICountryCatalogService catalogService, ICountryExporter exporter, IFileDialogService fileDialog)
    {
        _catalogService = catalogService;
        _exporter = exporter;
        _fileDialog = fileDialog;
    }

    public override string Title => UiText.AllCountriesTab;

    public ObservableCollection<CountryRow> Countries { get; } = [];

    protected override async Task LoadAsync()
    {
        var countries = await _catalogService.GetAllCountryDetailsAsync();
        _allRows = countries.Select(c => new CountryRow(c)).ToList();
        ApplyFilter();
    }

    partial void OnFilterChanged(string value) => ApplyFilter();

    [RelayCommand]
    private async Task ExportAsync()
    {
        var filePath = _fileDialog.AskSaveFilePath(UiText.CsvDefaultFileName, UiText.CsvFileFilter);
        if (filePath is null)
        {
            return;
        }

        var countries = Countries.Select(row => row.Country).ToList();
        StatusMessage = null;
        if (await RunAsync(() => _exporter.ExportAsync(countries, filePath)))
        {
            StatusMessage = UiText.ExportDone(countries.Count, filePath);
        }
    }

    private void ApplyFilter()
    {
        Countries.ReplaceWith(_allRows.Where(r => TextFilter.Matches(Filter, r.IsoCode, r.Name, r.CapitalCity, r.Continent, r.Currency, r.Languages)));
        StatusMessage = UiText.CountriesTotal(Countries.Count);
    }
}
