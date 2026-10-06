using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;
using CountryInfoApp.Presentation.ViewModels;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Presentation;

public class CountryDetailsViewModelTests
{
    private readonly FakeCountryCatalogService _catalog = new()
    {
        Countries =
        [
            TestData.Country("DE", "Germany", "Berlin", "EU", "EUR", new Language("de", "German")),
            TestData.Country("PL", "Poland", "Warsaw", "EU", "PLN"),
        ],
    };

    [Fact]
    public async Task LoadAsync_FillsCardAndExcludesCountryFromSameCurrencyList()
    {
        using var viewModel = new CountryDetailsViewModel(_catalog, new FakeCurrencyService());

        await viewModel.LoadAsync("DE");

        Assert.True(viewModel.HasCountry);
        Assert.Equal("Europa", viewModel.ContinentName);
        Assert.Equal("+1", viewModel.PhoneCode);
        Assert.Equal("Euro (EUR)", viewModel.Currency);
        Assert.Equal("German (de)", viewModel.Languages);
        Assert.Equal("France", Assert.Single(viewModel.CountriesWithSameCurrency).Name);
    }

    [Fact]
    public async Task LoadAsync_FallsBackToCurrencyCode_WhenNameUnknown()
    {
        using var viewModel = new CountryDetailsViewModel(_catalog, new FakeCurrencyService());

        await viewModel.LoadAsync("PL");

        Assert.Equal("PLN", viewModel.Currency);
        Assert.Null(viewModel.ErrorMessage);
    }

    [Fact]
    public async Task LoadAsync_ShowsNotFoundMessage_ForUnknownCountry()
    {
        using var viewModel = new CountryDetailsViewModel(_catalog, new FakeCurrencyService());

        await viewModel.LoadAsync("XX");

        Assert.Equal(UiText.NotFound, viewModel.ErrorMessage);
        Assert.False(viewModel.HasCountry);
    }
}
