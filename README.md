# CountryInfoApp

A WPF desktop application built on the public SOAP web service
[CountryInfoService](http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso).
The source code is in English; the user interface is in Polish.

## Features

| Tab (UI name) | What it does | Service operations |
|---|---|---|
| Przeglądaj | Continents → countries → country card with flag, capital, phone code, currency, languages and other countries sharing the currency | `ListOfContinentsByName/ByCode`, `ListOfCountryNamesGroupedByContinent`, `FullCountryInfo`, `CurrencyName`, `CountriesUsingCurrency` |
| Wyszukiwarka | Quick lookups: name ↔ ISO code, capital, phone code, country currency, currency name, language name ↔ ISO code | `CountryISOCode`, `CountryName`, `CapitalCity`, `CountryIntPhoneCode`, `CountryCurrency`, `CurrencyName`, `LanguageISOCode`, `LanguageName` |
| Wszystkie kraje | Filterable table of all countries, CSV export | `FullCountryInfoAllCountries` |
| Waluty | Currency list (sortable by name/code) and countries using the selected currency | `ListOfCurrenciesByName/ByCode`, `CountriesUsingCurrency` |
| Języki | Language list and countries where the selected language is used | `ListOfLanguagesByName`, `FullCountryInfoAllCountries` |
| Statystyki | Countries per continent, most common currencies and languages | `FullCountryInfoAllCountries` |
| Quiz | Guess the capital, the flag or the currency, with a score | `FullCountryInfoAllCountries` |

Responses are cached in memory, every call is asynchronous (the window never freezes),
and network or "not found" errors are shown as a Polish message with a retry button.

## Running

Requirements: Windows and the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0).

```
dotnet run --project src/CountryInfoApp.Wpf
```

or open `CountryInfoApp.sln` in Visual Studio 2022 and start `CountryInfoApp.Wpf`.

Tests (run on any OS):

```
dotnet test
```

## Architecture

```
CountryInfoApp.Core            Domain models, service interfaces, pure logic (statistics, quiz). No dependencies.
CountryInfoApp.Infrastructure  SOAP client, caching decorator, XML mapping, service implementations, CSV export.
CountryInfoApp.Presentation    MVVM view models (CommunityToolkit.Mvvm) and Polish UI texts. No WPF dependency.
CountryInfoApp.Wpf             Views (XAML), WPF-specific services, composition root (App.xaml.cs).
tests/CountryInfoApp.Tests     xUnit tests for all layers except the XAML views.
```

Dependencies point inwards: `Wpf → Presentation/Infrastructure → Core`.

### How the principles are applied

- **Single responsibility**: the SOAP transport (`SoapClient`), caching (`CachingSoapClient`), XML mapping
  (`CountryInfoXmlMapper`) and each service area are separate classes; each tab has its own view model.
- **Open/closed**: caching is added as a decorator without touching `SoapClient`; a new quick lookup is one
  entry in `LookupOptionsProvider`; a new tab is one view model, one view and one `AddTab` line.
- **Liskov substitution**: every service is consumed through its interface, so tests swap in fakes freely.
- **Interface segregation**: small interfaces per area (`IContinentService`, `ICountryCatalogService`,
  `ICountryLookupService`, `ICurrencyService`, `ILanguageService`) instead of one large service interface.
- **Dependency inversion**: view models depend on Core abstractions; concrete types are wired only in the
  composition root through `Microsoft.Extensions.DependencyInjection`.
- **KISS / YAGNI**: a hand-written ~100-line SOAP client instead of generated proxy code, in-memory cache,
  simple bar charts built from standard WPF controls, no extra frameworks.
- **DRY**: shared `ViewModelBase.RunAsync` for busy state and error handling, shared status template in XAML.

Build settings (`Directory.Build.props`) enable nullable reference types, the recommended .NET analyzers
and treat all warnings as errors.

## Notes

- Country, capital, currency and language names come from the service and are therefore in English;
  continent names are translated to Polish in `ContinentNames`.
- The service endpoint and timeout are configured in `SoapClientOptions`.
