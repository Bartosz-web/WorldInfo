using System.Xml.Linq;
using CountryInfoApp.Infrastructure.Soap;

namespace CountryInfoApp.Tests.Fakes;

internal sealed class FakeSoapClient : ISoapClient
{
    private readonly Func<string, IReadOnlyDictionary<string, string>, XElement> _respond;

    public FakeSoapClient(Func<string, IReadOnlyDictionary<string, string>, XElement> respond)
    {
        _respond = respond;
    }

    public List<(string Operation, IReadOnlyDictionary<string, string> Parameters)> Calls { get; } = [];

    public Task<XElement> InvokeAsync(string operation, IReadOnlyDictionary<string, string> parameters, CancellationToken cancellationToken = default)
    {
        Calls.Add((operation, parameters));
        return Task.FromResult(_respond(operation, parameters));
    }
}
