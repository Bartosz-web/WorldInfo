using System.Net.Http.Headers;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CountryInfoApp.Core.Exceptions;

namespace CountryInfoApp.Infrastructure.Soap;

/// <summary>
/// Minimal SOAP 1.1 client: builds the envelope, posts it and unwraps the result element.
/// </summary>
public sealed class SoapClient : ISoapClient
{
    private static readonly XNamespace EnvelopeNamespace = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly MediaTypeHeaderValue XmlContentType = new("text/xml") { CharSet = "utf-8" };

    private readonly HttpClient _httpClient;
    private readonly SoapClientOptions _options;
    private readonly XNamespace _serviceNamespace;

    public SoapClient(HttpClient httpClient, SoapClientOptions options)
    {
        _httpClient = httpClient;
        _options = options;
        _serviceNamespace = options.ServiceNamespace;
    }

    public async Task<XElement> InvokeAsync(
        string operation,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, _options.Endpoint)
        {
            Content = new StringContent(BuildEnvelope(operation, parameters).ToString(SaveOptions.DisableFormatting), Encoding.UTF8),
        };
        request.Content.Headers.ContentType = XmlContentType;
        request.Headers.Add("SOAPAction", "\"\"");

        var responseBody = await SendAsync(request, operation, cancellationToken).ConfigureAwait(false);
        return ExtractResult(ParseResponse(responseBody, operation), operation);
    }

    private XDocument BuildEnvelope(string operation, IReadOnlyDictionary<string, string> parameters) =>
        new(
            new XElement(
                EnvelopeNamespace + "Envelope",
                new XAttribute(XNamespace.Xmlns + "soap", EnvelopeNamespace),
                new XElement(
                    EnvelopeNamespace + "Body",
                    new XElement(
                        _serviceNamespace + operation,
                        parameters.Select(p => new XElement(_serviceNamespace + p.Key, p.Value))))));

    private async Task<string> SendAsync(HttpRequestMessage request, string operation, CancellationToken cancellationToken)
    {
        try
        {
            using var response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            // SOAP faults come back with HTTP 500, so the body is inspected before the status code.
            if (!response.IsSuccessStatusCode && !body.Contains("Fault", StringComparison.Ordinal))
            {
                throw new CountryInfoServiceException($"Operation '{operation}' failed with HTTP {(int)response.StatusCode}.");
            }

            return body;
        }
        catch (HttpRequestException ex)
        {
            throw new CountryInfoServiceException($"Operation '{operation}' could not reach the service.", ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            throw new CountryInfoServiceException($"Operation '{operation}' timed out.", ex);
        }
    }

    private static XDocument ParseResponse(string body, string operation)
    {
        try
        {
            return XDocument.Parse(body);
        }
        catch (XmlException ex)
        {
            throw new CountryInfoServiceException($"Operation '{operation}' returned invalid XML.", ex);
        }
    }

    private XElement ExtractResult(XDocument document, string operation)
    {
        var fault = document.Descendants(EnvelopeNamespace + "Fault").FirstOrDefault();
        if (fault is not null)
        {
            var reason = fault.Element("faultstring")?.Value ?? "Unknown SOAP fault.";
            throw new CountryInfoServiceException($"Operation '{operation}' returned a fault: {reason}");
        }

        return document.Descendants(_serviceNamespace + operation + "Result").FirstOrDefault()
            ?? throw new CountryInfoServiceException($"Operation '{operation}' returned no result element.");
    }
}
