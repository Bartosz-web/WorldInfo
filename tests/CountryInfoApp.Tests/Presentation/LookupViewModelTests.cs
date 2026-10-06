using CountryInfoApp.Presentation.Lookups;
using CountryInfoApp.Presentation.ViewModels;

namespace CountryInfoApp.Tests.Presentation;

public class LookupViewModelTests
{
    private sealed class StubOptionsProvider : ILookupOptionsProvider
    {
        public IReadOnlyList<LookupOption> GetOptions() =>
        [
            new("Upper", "hint", (input, _) => Task.FromResult(input.ToUpperInvariant())),
            new("Length", "hint", (input, _) => Task.FromResult(input.Length.ToString(System.Globalization.CultureInfo.InvariantCulture))),
        ];
    }

    [Fact]
    public void SearchCommand_IsDisabled_WhenInputEmpty()
    {
        var viewModel = new LookupViewModel(new StubOptionsProvider());

        Assert.False(viewModel.SearchCommand.CanExecute(null));

        viewModel.Input = "pl";
        Assert.True(viewModel.SearchCommand.CanExecute(null));
    }

    [Fact]
    public async Task SearchCommand_RunsSelectedOptionWithTrimmedInput()
    {
        var viewModel = new LookupViewModel(new StubOptionsProvider()) { Input = "  poland " };
        viewModel.SelectedOption = viewModel.Options[1];

        await viewModel.SearchCommand.ExecuteAsync(null);

        Assert.Equal("6", viewModel.Result);
    }

    [Fact]
    public async Task ChangingOption_ClearsResult()
    {
        var viewModel = new LookupViewModel(new StubOptionsProvider()) { Input = "pl" };
        await viewModel.SearchCommand.ExecuteAsync(null);

        viewModel.SelectedOption = viewModel.Options[1];

        Assert.Null(viewModel.Result);
    }
}
