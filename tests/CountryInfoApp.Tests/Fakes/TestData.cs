using System.Xml.Linq;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Tests.Fakes;

internal static class TestData
{
    public const string Namespace = "http://www.oorsprong.org/websamples.countryinfo";

    public static readonly XNamespace Ns = Namespace;

    public static CountryDetails Country(
        string isoCode,
        string name,
        string capital = "Capital",
        string continent = "EU",
        string currency = "EUR",
        params Language[] languages) =>
        new(isoCode, name, capital, "1", continent, currency, $"http://flags/{name}.jpg", languages);

    public static XElement Result(string operation, params object[] content) =>
        new(Ns + (operation + "Result"), content);

    public static XElement Element(string name, params object[] content) => new(Ns + name, content);

    public static string Envelope(XElement body) =>
        new XElement(
            XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/") + "Envelope",
            new XElement(XNamespace.Get("http://schemas.xmlsoap.org/soap/envelope/") + "Body", body)).ToString();
}
