namespace CountryInfoApp.Infrastructure.Soap;

public sealed class SoapClientOptions
{
    public Uri Endpoint { get; init; } =
        new("http://webservices.oorsprong.org/websamples.countryinfo/CountryInfoService.wso");

    public string ServiceNamespace { get; init; } = "http://www.oorsprong.org/websamples.countryinfo";

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
}
