using System.Net;
using System.Xml.Linq;
using CountryInfoApp.Core.Exceptions;
using CountryInfoApp.Infrastructure.Soap;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Infrastructure;

public class SoapClientTests
{
    private static readonly SoapClientOptions Options = new();

    [Fact]
    public async Task InvokeAsync_SendsEnvelopeWithOperationAndParameters()
    {
        var response = TestData.Envelope(TestData.Element("CapitalCityResponse", TestData.Result("CapitalCity", "Warsaw")));
        var handler = new FakeHttpMessageHandler(HttpStatusCode.OK, response);
        using var httpClient = new HttpClient(handler);
        var client = new SoapClient(httpClient, Options);

        await client.InvokeAsync("CapitalCity", new Dictionary<string, string> { ["sCountryISOCode"] = "PL" });

        var body = XDocument.Parse(handler.LastRequestBody!);
        var operation = body.Descendants(TestData.Ns + "CapitalCity").Single();
        Assert.Equal("PL", operation.Element(TestData.Ns + "sCountryISOCode")!.Value);
        Assert.Equal(HttpMethod.Post, handler.LastRequest!.Method);
        Assert.Equal("text/xml", handler.LastRequest.Content!.Headers.ContentType!.MediaType);
    }

    [Fact]
    public async Task InvokeAsync_ReturnsResultElement()
    {
        var response = TestData.Envelope(TestData.Element("CapitalCityResponse", TestData.Result("CapitalCity", "Warsaw")));
        using var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, response));
        var client = new SoapClient(httpClient, Options);

        var result = await client.InvokeAsync("CapitalCity", new Dictionary<string, string>());

        Assert.Equal("Warsaw", result.Value);
    }

    [Fact]
    public async Task InvokeAsync_ThrowsServiceException_OnSoapFault()
    {
        const string fault = """
            <soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
              <soap:Body><soap:Fault><faultcode>soap:Server</faultcode><faultstring>Boom</faultstring></soap:Fault></soap:Body>
            </soap:Envelope>
            """;
        using var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.InternalServerError, fault));
        var client = new SoapClient(httpClient, Options);

        var ex = await Assert.ThrowsAsync<CountryInfoServiceException>(() => client.InvokeAsync("CapitalCity", new Dictionary<string, string>()));
        Assert.Contains("Boom", ex.Message, StringComparison.Ordinal);
    }

    [Fact]
    public async Task InvokeAsync_ThrowsServiceException_WhenNetworkFails()
    {
        using var httpClient = new HttpClient(new ThrowingHttpMessageHandler());
        var client = new SoapClient(httpClient, Options);

        await Assert.ThrowsAsync<CountryInfoServiceException>(() => client.InvokeAsync("CapitalCity", new Dictionary<string, string>()));
    }

    [Fact]
    public async Task InvokeAsync_ThrowsServiceException_OnInvalidXml()
    {
        using var httpClient = new HttpClient(new FakeHttpMessageHandler(HttpStatusCode.OK, "not xml"));
        var client = new SoapClient(httpClient, Options);

        await Assert.ThrowsAsync<CountryInfoServiceException>(() => client.InvokeAsync("CapitalCity", new Dictionary<string, string>()));
    }
}
