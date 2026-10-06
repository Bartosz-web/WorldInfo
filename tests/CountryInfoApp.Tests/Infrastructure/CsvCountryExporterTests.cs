using CountryInfoApp.Core.Models;
using CountryInfoApp.Infrastructure.Export;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Infrastructure;

public class CsvCountryExporterTests
{
    [Fact]
    public async Task WriteAsync_WritesHeaderAndRows()
    {
        using var writer = new StringWriter();

        await CsvCountryExporter.WriteAsync([TestData.Country("PL", "Poland", "Warsaw", "EU", "PLN", new Language("pl", "Polish"))], writer);

        var lines = writer.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.Equal(2, lines.Length);
        Assert.StartsWith("IsoCode;Name;CapitalCity", lines[0], StringComparison.Ordinal);
        Assert.StartsWith("PL;Poland;Warsaw;1;EU;PLN;Polish;", lines[1], StringComparison.Ordinal);
    }

    [Fact]
    public async Task WriteAsync_QuotesValuesContainingSeparatorOrQuotes()
    {
        using var writer = new StringWriter();

        await CsvCountryExporter.WriteAsync([TestData.Country("XX", "Some; \"Land\"")], writer);

        Assert.Contains("\"Some; \"\"Land\"\"\"", writer.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task ExportAsync_CreatesFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
        try
        {
            await new CsvCountryExporter().ExportAsync([TestData.Country("PL", "Poland")], path);

            Assert.Contains("Poland", await File.ReadAllTextAsync(path), StringComparison.Ordinal);
        }
        finally
        {
            File.Delete(path);
        }
    }
}
