using System.Xml.Linq;

namespace CountryInfoApp.Infrastructure.Soap;

public interface ISoapClient
{
    /// <summary>
    /// Calls a SOAP operation and returns its <c>{operation}Result</c> element.
    /// </summary>
    Task<XElement> InvokeAsync(
        string operation,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken = default);
}
