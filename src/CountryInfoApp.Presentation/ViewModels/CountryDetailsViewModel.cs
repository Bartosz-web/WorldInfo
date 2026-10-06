using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

/// <summary>
/// The country card: full information, flag and other countries sharing the currency.
/// </summary>
public sealed partial class CountryDetailsViewModel : ViewModelBase, IDisposable
{
    private readonly ICountryCatalogService _catalogService;
    private readonly ICurrencyService _currencyService;
    private CancellationTokenSource? _loadCancellation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCountry), nameof(ContinentName), nameof(PhoneCode), nameof(Languages))]
    private CountryDetails? _country;

    [ObservableProperty]
    private string _currency = string.Empty;

    public CountryDetailsViewModel(ICountryCatalogService catalogService, ICurrencyService currencyService)
    {
        _catalogService = catalogService;
        _currencyService = currencyService;
    }

    public bool HasCountry => Country is not null;

    public string ContinentName => Country is null ? string.Empty : ContinentNames.ToPolish(Country.ContinentCode);

    public string PhoneCode => Country is null ? string.Empty : UiText.PhoneCode(Country.PhoneCode);

    public string Languages => Country is null
        ? string.Empty
        : string.Join(", ", Country.Languages.Select(l => UiText.CodeAndName(l.IsoCode, l.Name)));

    public ObservableCollection<CountrySummary> CountriesWithSameCurrency { get; } = [];

    public async Task LoadAsync(string countryIsoCode)
    {
        // A newer selection makes the previous, still running load irrelevant.
        if (_loadCancellation is not null)
        {
            await _loadCancellation.CancelAsync();
            _loadCancellation.Dispose();
        }

        _loadCancellation = new CancellationTokenSource();
        var token = _loadCancellation.Token;

        await RunAsync(async () =>
        {
            var details = await _catalogService.GetCountryDetailsAsync(countryIsoCode, token);
            var currencyName = await TryGetCurrencyNameAsync(details.CurrencyIsoCode, token);
            var sameCurrency = await GetCountriesWithSameCurrencyAsync(details, token);
            token.ThrowIfCancellationRequested();

            Country = details;
            Currency = currencyName is null ? details.CurrencyIsoCode : UiText.CodeAndName(details.CurrencyIsoCode, currencyName);
            CountriesWithSameCurrency.ReplaceWith(sameCurrency);
        });
    }

    private async Task<string?> TryGetCurrencyNameAsync(string currencyIsoCode, CancellationToken token)
    {
        try
        {
            return await _currencyService.GetCurrencyNameAsync(currencyIsoCode, token);
        }
        catch (EntityNotFoundException)
        {
            return null;
        }
    }

    private async Task<IEnumerable<CountrySummary>> GetCountriesWithSameCurrencyAsync(CountryDetails details, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(details.CurrencyIsoCode))
        {
            return [];
        }

        var countries = await _currencyService.GetCountriesUsingCurrencyAsync(details.CurrencyIsoCode, token);
        return countries.Where(c => !string.Equals(c.IsoCode, details.IsoCode, StringComparison.OrdinalIgnoreCase));
    }

    public void Dispose()
    {
        _loadCancellation?.Cancel();
        _loadCancellation?.Dispose();
        _loadCancellation = null;
    }
}
