using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Services;
using CountryInfoApp.Infrastructure.Soap;
using CountryInfoApp.Tests.Fakes;
using static CountryInfoApp.Tests.Fakes.TestData;

namespace CountryInfoApp.Tests.Infrastructure;

public class SoapServicesTests
{
    private readonly CountryInfoXmlMapper _mapper = new(new SoapClientOptions());

    [Theory]
    [InlineData(SortOrder.ByName, "ListOfContinentsByName")]
    [InlineData(SortOrder.ByCode, "ListOfContinentsByCode")]
    public async Task GetContinentsAsync_UsesOperationMatchingSortOrder(SortOrder sortOrder, string expectedOperation)
    {
        var client = new FakeSoapClient((operation, _) => Result(operation));
        var service = new SoapContinentService(client, _mapper);

        await service.GetContinentsAsync(sortOrder);

        Assert.Equal(expectedOperation, Assert.Single(client.Calls).Operation);
    }

    [Fact]
    public async Task GetCapitalCityAsync_SendsUpperCaseIsoCode()
    {
        var client = new FakeSoapClient((operation, _) => Result(operation, "Warsaw"));
        var service = new SoapCountryLookupService(client, _mapper);

        var capital = await service.GetCapitalCityAsync(" pl ");

        Assert.Equal("Warsaw", capital);
        var call = Assert.Single(client.Calls);
        Assert.Equal("CapitalCity", call.Operation);
        Assert.Equal("PL", call.Parameters["sCountryISOCode"]);
    }

    [Fact]
    public async Task GetCountriesUsingCurrencyAsync_MapsCountries()
    {
        var client = new FakeSoapClient((operation, _) => Result(
            operation,
            Element("tCountryCodeAndName", Element("sISOCode", "DE"), Element("sName", "Germany"))));
        var service = new SoapCurrencyService(client, _mapper);

        var countries = await service.GetCountriesUsingCurrencyAsync("eur");

        Assert.Equal("Germany", Assert.Single(countries).Name);
        Assert.Equal("EUR", client.Calls[0].Parameters["sISOCurrencyCode"]);
    }
}
