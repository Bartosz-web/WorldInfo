using System.Collections.Concurrent;
using System.Xml.Linq;

namespace CountryInfoApp.Infrastructure.Soap;

/// <summary>
/// Decorator that keeps successful responses in memory; the reference data rarely changes.
/// </summary>
public sealed class CachingSoapClient : ISoapClient
{
    private readonly ISoapClient _inner;
    private readonly ConcurrentDictionary<string, XElement> _cache = new(StringComparer.Ordinal);

    public CachingSoapClient(ISoapClient inner)
    {
        _inner = inner;
    }

    public async Task<XElement> InvokeAsync(
        string operation,
        IReadOnlyDictionary<string, string> parameters,
        CancellationToken cancellationToken = default)
    {
        var key = BuildKey(operation, parameters);
        if (_cache.TryGetValue(key, out var cached))
        {
            return new XElement(cached);
        }

        var result = await _inner.InvokeAsync(operation, parameters, cancellationToken).ConfigureAwait(false);
        _cache[key] = result;
        return new XElement(result);
    }

    private static string BuildKey(string operation, IReadOnlyDictionary<string, string> parameters) =>
        parameters
            .OrderBy(p => p.Key, StringComparer.Ordinal)
            .Aggregate(operation, (key, p) => $"{key}|{p.Key}={p.Value}");
}
