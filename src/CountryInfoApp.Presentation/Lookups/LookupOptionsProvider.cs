using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.Lookups;

/// <summary>
/// Defines the available lookups. Adding a new one means adding one entry here.
/// </summary>
public sealed class LookupOptionsProvider : ILookupOptionsProvider
{
    private readonly ICountryLookupService _countries;
    private readonly ICurrencyService _currencies;
    private readonly ILanguageService _languages;

    public LookupOptionsProvider(ICountryLookupService countries, ICurrencyService currencies, ILanguageService languages)
    {
        _countries = countries;
        _currencies = currencies;
        _languages = languages;
    }

    public IReadOnlyList<LookupOption> GetOptions() =>
    [
        new(UiText.LookupCountryIsoCode, UiText.HintCountryName, _countries.GetCountryIsoCodeAsync),
        new(UiText.LookupCountryName, UiText.HintCountryIsoCode, _countries.GetCountryNameAsync),
        new(UiText.LookupCapitalCity, UiText.HintCountryIsoCode, _countries.GetCapitalCityAsync),
        new(UiText.LookupPhoneCode, UiText.HintCountryIsoCode, async (input, ct) => UiText.PhoneCode(await _countries.GetPhoneCodeAsync(input, ct))),
        new(UiText.LookupCountryCurrency, UiText.HintCountryIsoCode, async (input, ct) =>
        {
            var currency = await _countries.GetCountryCurrencyAsync(input, ct);
            return UiText.CodeAndName(currency.IsoCode, currency.Name);
        }),
        new(UiText.LookupCurrencyName, UiText.HintCurrencyIsoCode, _currencies.GetCurrencyNameAsync),
        new(UiText.LookupLanguageIsoCode, UiText.HintLanguageName, _languages.GetLanguageIsoCodeAsync),
        new(UiText.LookupLanguageName, UiText.HintLanguageIsoCode, _languages.GetLanguageNameAsync),
    ];
}
