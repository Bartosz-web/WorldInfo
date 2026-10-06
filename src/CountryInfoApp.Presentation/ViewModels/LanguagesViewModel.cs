using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class LanguagesViewModel : TabViewModelBase
{
    private readonly ILanguageService _languageService;
    private readonly ICountryCatalogService _catalogService;
    private IReadOnlyList<Language> _allLanguages = [];
    private IReadOnlyList<CountryDetails> _allCountries = [];

    [ObservableProperty]
    private string _filter = string.Empty;

    [ObservableProperty]
    private Language? _selectedLanguage;

    public LanguagesViewModel(ILanguageService languageService, ICountryCatalogService catalogService)
    {
        _languageService = languageService;
        _catalogService = catalogService;
    }

    public override string Title => UiText.LanguagesTab;

    public ObservableCollection<Language> Languages { get; } = [];

    public ObservableCollection<CountryDetails> CountriesSpeakingLanguage { get; } = [];

    protected override async Task LoadAsync()
    {
        _allLanguages = await _languageService.GetLanguagesAsync(SortOrder.ByName);

        // The service has no "countries by language" operation, so it is derived from full country data.
        _allCountries = await _catalogService.GetAllCountryDetailsAsync();
        ApplyFilter();
    }

    partial void OnFilterChanged(string value) => ApplyFilter();

    partial void OnSelectedLanguageChanged(Language? value) =>
        CountriesSpeakingLanguage.ReplaceWith(value is null ? [] : _allCountries.Where(c => c.Languages.Any(l => IsSameLanguage(l, value))));

    private static bool IsSameLanguage(Language left, Language right) =>
        string.Equals(left.IsoCode, right.IsoCode, StringComparison.OrdinalIgnoreCase)
        || string.Equals(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);

    private void ApplyFilter() =>
        Languages.ReplaceWith(_allLanguages.Where(l => TextFilter.Matches(Filter, l.IsoCode, l.Name)));
}
