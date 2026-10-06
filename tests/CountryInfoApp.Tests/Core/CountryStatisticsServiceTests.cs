using CountryInfoApp.Core.Models;
using CountryInfoApp.Core.Services;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Core;

public class CountryStatisticsServiceTests
{
    private static readonly Language English = new("en", "English");
    private static readonly Language French = new("fr", "French");

    private static readonly CountryDetails[] Countries =
    [
        TestData.Country("FR", "France", continent: "EU", currency: "EUR", languages: French),
        TestData.Country("DE", "Germany", continent: "EU", currency: "EUR"),
        TestData.Country("CA", "Canada", continent: "AM", currency: "CAD", languages: [English, French]),
        TestData.Country("US", "United States", continent: "AM", currency: "USD", languages: English),
        TestData.Country("GB", "United Kingdom", continent: "EU", currency: "GBP", languages: English),
    ];

    private readonly CountryStatisticsService _service = new();

    [Fact]
    public void CountByContinent_ReturnsLargestFirst()
    {
        var result = _service.CountByContinent(Countries);

        Assert.Equal([new LabeledCount("EU", 3), new LabeledCount("AM", 2)], result);
    }

    [Fact]
    public void TopCurrencies_TakesRequestedNumber()
    {
        var result = _service.TopCurrencies(Countries, 1);

        Assert.Equal(new LabeledCount("EUR", 2), Assert.Single(result));
    }

    [Fact]
    public void TopLanguages_CountsEveryCountrySpeakingLanguage()
    {
        var result = _service.TopLanguages(Countries, 10);

        Assert.Equal([new LabeledCount("English", 3), new LabeledCount("French", 2)], result);
    }
}
