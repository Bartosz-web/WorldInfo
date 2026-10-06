using CountryInfoApp.Infrastructure.Soap;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Infrastructure;

public class CachingSoapClientTests
{
    [Fact]
    public async Task InvokeAsync_CallsInnerClientOnce_ForSameRequest()
    {
        var inner = new FakeSoapClient((operation, _) => TestData.Result(operation, "value"));
        var client = new CachingSoapClient(inner);
        var parameters = new Dictionary<string, string> { ["sCountryISOCode"] = "PL" };

        var first = await client.InvokeAsync("CapitalCity", parameters);
        var second = await client.InvokeAsync("CapitalCity", parameters);

        Assert.Single(inner.Calls);
        Assert.Equal(first.Value, second.Value);
    }

    [Fact]
    public async Task InvokeAsync_CallsInnerClient_ForDifferentParameters()
    {
        var inner = new FakeSoapClient((operation, _) => TestData.Result(operation, "value"));
        var client = new CachingSoapClient(inner);

        await client.InvokeAsync("CapitalCity", new Dictionary<string, string> { ["sCountryISOCode"] = "PL" });
        await client.InvokeAsync("CapitalCity", new Dictionary<string, string> { ["sCountryISOCode"] = "DE" });

        Assert.Equal(2, inner.Calls.Count);
    }
}
