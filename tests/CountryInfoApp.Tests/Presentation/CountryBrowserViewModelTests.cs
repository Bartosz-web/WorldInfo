using CountryInfoApp.Presentation.Localization;
using CountryInfoApp.Presentation.ViewModels;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Presentation;

public class CountryBrowserViewModelTests
{
    private readonly FakeCountryCatalogService _catalog = new()
    {
        Countries =
        [
            TestData.Country("PL", "Poland", "Warsaw", "EU", "PLN"),
            TestData.Country("DE", "Germany", "Berlin", "EU", "EUR"),
            TestData.Country("EG", "Egypt", "Cairo", "AF", "EGP"),
        ],
    };

    private CountryBrowserViewModel CreateViewModel() =>
        new(new FakeContinentService(), _catalog, new CountryDetailsViewModel(_catalog, new FakeCurrencyService()));

    [Fact]
    public async Task EnsureLoadedAsync_LoadsContinentsInPolishAndSelectsFirst()
    {
        var viewModel = CreateViewModel();

        await viewModel.EnsureLoadedAsync();

        Assert.Equal(["Afryka (AF)", "Europa (EU)"], viewModel.Continents.Select(c => c.DisplayName));
        Assert.Equal("AF", viewModel.SelectedContinent!.Code);
        Assert.Equal("Egypt", Assert.Single(viewModel.Countries).Name);
    }

    [Fact]
    public async Task SelectingContinent_ShowsItsCountriesSortedByName()
    {
        var viewModel = CreateViewModel();
        await viewModel.EnsureLoadedAsync();

        viewModel.SelectedContinent = viewModel.Continents.Single(c => c.Code == "EU");

        Assert.Equal(["Germany", "Poland"], viewModel.Countries.Select(c => c.Name));
    }

    [Fact]
    public async Task CountryFilter_NarrowsCountries()
    {
        var viewModel = CreateViewModel();
        await viewModel.EnsureLoadedAsync();
        viewModel.SelectedContinent = viewModel.Continents.Single(c => c.Code == "EU");

        viewModel.CountryFilter = "pol";

        Assert.Equal("Poland", Assert.Single(viewModel.Countries).Name);
    }

    [Fact]
    public async Task EnsureLoadedAsync_ShowsPolishError_WhenServiceFails()
    {
        _catalog.ShouldFail = true;
        var viewModel = CreateViewModel();

        await viewModel.EnsureLoadedAsync();

        Assert.Equal(UiText.ServiceUnavailable, viewModel.ErrorMessage);
        Assert.False(viewModel.IsBusy);
    }

    [Fact]
    public async Task RetryCommand_ReloadsAfterFailure()
    {
        _catalog.ShouldFail = true;
        var viewModel = CreateViewModel();
        await viewModel.EnsureLoadedAsync();

        _catalog.ShouldFail = false;
        await viewModel.RetryCommand.ExecuteAsync(null);

        Assert.Null(viewModel.ErrorMessage);
        Assert.NotEmpty(viewModel.Continents);
    }
}
