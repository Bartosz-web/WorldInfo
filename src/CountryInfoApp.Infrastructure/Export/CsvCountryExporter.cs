using System.Text;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Infrastructure.Export;

public sealed class CsvCountryExporter : ICountryExporter
{
    private const char Separator = ';';

    private static readonly string[] Header =
        ["IsoCode", "Name", "CapitalCity", "PhoneCode", "ContinentCode", "CurrencyIsoCode", "Languages", "FlagUrl"];

    public async Task ExportAsync(IEnumerable<CountryDetails> countries, string filePath, CancellationToken cancellationToken = default)
    {
        // UTF-8 with BOM so that Excel recognises the encoding.
        await using var writer = new StreamWriter(filePath, append: false, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));
        await WriteAsync(countries, writer, cancellationToken).ConfigureAwait(false);
    }

    public static async Task WriteAsync(IEnumerable<CountryDetails> countries, TextWriter writer, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(countries);
        ArgumentNullException.ThrowIfNull(writer);

        await writer.WriteLineAsync(FormatLine(Header).AsMemory(), cancellationToken).ConfigureAwait(false);
        foreach (var country in countries)
        {
            var line = FormatLine(
            [
                country.IsoCode,
                country.Name,
                country.CapitalCity,
                country.PhoneCode,
                country.ContinentCode,
                country.CurrencyIsoCode,
                string.Join(", ", country.Languages.Select(l => l.Name)),
                country.FlagUrl,
            ]);
            await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
        }
    }

    private static string FormatLine(IEnumerable<string> values) => string.Join(Separator, values.Select(Escape));

    private static string Escape(string value) =>
        value.IndexOfAny([Separator, '"', '\n', '\r']) >= 0
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;
}
