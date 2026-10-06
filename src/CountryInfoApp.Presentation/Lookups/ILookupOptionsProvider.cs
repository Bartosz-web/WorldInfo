namespace CountryInfoApp.Presentation.Lookups;

public interface ILookupOptionsProvider
{
    IReadOnlyList<LookupOption> GetOptions();
}
