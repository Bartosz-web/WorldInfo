using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Infrastructure.Soap;
using CountryInfoApp.Tests.Fakes;
using static CountryInfoApp.Tests.Fakes.TestData;

namespace CountryInfoApp.Tests.Infrastructure;

public class CountryInfoXmlMapperTests
{
    private readonly CountryInfoXmlMapper _mapper = new(new SoapClientOptions());

    [Fact]
    public void ToContinents_MapsCodeAndName()
    {
        var result = Result(
            "ListOfContinentsByName",
            Element("tContinent", Element("sCode", "AF"), Element("sName", "Africa")),
            Element("tContinent", Element("sCode", "EU"), Element("sName", "Europe")));

        var continents = _mapper.ToContinents(result);

        Assert.Equal(["AF", "EU"], continents.Select(c => c.Code));
        Assert.Equal("Europe", continents[1].Name);
    }

    [Fact]
    public void ToCountryDetails_MapsAllFieldsAndLanguages()
    {
        var element = Element(
            "tCountryInfo",
            Element("sISOCode", "PL"),
            Element("sName", "Poland"),
            Element("sCapitalCity", "Warsaw"),
            Element("sPhoneCode", "48"),
            Element("sContinentCode", "EU"),
            Element("sCurrencyISOCode", "PLN"),
            Element("sCountryFlag", "http://flags/Poland.jpg"),
            Element("Languages", Element("tLanguage", Element("sISOCode", "pl"), Element("sName", "Polish"))));

        var details = _mapper.ToCountryDetails(element);

        Assert.Equal("PL", details.IsoCode);
        Assert.Equal("Warsaw", details.CapitalCity);
        Assert.Equal("48", details.PhoneCode);
        Assert.Equal("PLN", details.CurrencyIsoCode);
        Assert.Equal("Polish", Assert.Single(details.Languages).Name);
    }

    [Fact]
    public void ToContinentCountries_MapsGroups()
    {
        var result = Result(
            "ListOfCountryNamesGroupedByContinent",
            Element(
                "tCountryCodeAndNameGroupedByContinent",
                Element("Continent", Element("sCode", "EU"), Element("sName", "Europe")),
                Element(
                    "CountryCodeAndNames",
                    Element("tCountryCodeAndName", Element("sISOCode", "PL"), Element("sName", "Poland")),
                    Element("tCountryCodeAndName", Element("sISOCode", "DE"), Element("sName", "Germany")))));

        var group = Assert.Single(_mapper.ToContinentCountries(result));

        Assert.Equal("EU", group.Continent.Code);
        Assert.Equal(2, group.Countries.Count);
    }

    [Fact]
    public void ToCountryDetails_Throws_WhenCountryNotFound()
    {
        var element = Element("tCountryInfo", Element("sName", "Country not found in the database"));

        Assert.Throws<EntityNotFoundException>(() => _mapper.ToCountryDetails(element));
    }

    [Theory]
    [InlineData("Country not found in the database")]
    [InlineData("No country found by that name")]
    [InlineData("")]
    public void ToText_Throws_ForNotFoundMessages(string value)
    {
        Assert.Throws<EntityNotFoundException>(() => CountryInfoXmlMapper.ToText(Result("CountryName", value)));
    }

    [Fact]
    public void ToText_ReturnsTrimmedValue()
    {
        Assert.Equal("Warsaw", CountryInfoXmlMapper.ToText(Result("CapitalCity", "  Warsaw ")));
    }
}
