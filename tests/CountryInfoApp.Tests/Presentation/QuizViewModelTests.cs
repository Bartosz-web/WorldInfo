using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;
using CountryInfoApp.Presentation.ViewModels;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Presentation;

public class QuizViewModelTests
{
    private sealed class FixedQuizService : IQuizService
    {
        public QuizQuestion CreateQuestion(IReadOnlyList<CountryDetails> countries) =>
            new(QuizQuestionType.Capital, countries[0], ["Berlin", "Warsaw", "Paris", "Prague"], "Warsaw");
    }

    private static async Task<QuizViewModel> CreateLoadedViewModelAsync()
    {
        var catalog = new FakeCountryCatalogService { Countries = [TestData.Country("PL", "Poland", "Warsaw")] };
        var viewModel = new QuizViewModel(catalog, new FixedQuizService());
        await viewModel.EnsureLoadedAsync();
        return viewModel;
    }

    [Fact]
    public async Task Load_ShowsPolishQuestion()
    {
        var viewModel = await CreateLoadedViewModelAsync();

        Assert.Equal(UiText.QuizCapitalQuestion("Poland"), viewModel.QuestionText);
        Assert.Equal(4, viewModel.Options.Count);
        Assert.Null(viewModel.FlagUrl);
    }

    [Fact]
    public async Task Answer_Correct_IncreasesScore()
    {
        var viewModel = await CreateLoadedViewModelAsync();

        viewModel.AnswerCommand.Execute(viewModel.Options.Single(o => o.Text == "Warsaw"));

        Assert.Equal(UiText.QuizScore(1, 1), viewModel.Score);
        Assert.Equal(UiText.QuizCorrect, viewModel.Feedback);
        Assert.Equal(AnswerState.Correct, viewModel.Options.Single(o => o.Text == "Warsaw").State);
    }

    [Fact]
    public async Task Answer_Wrong_MarksBothOptionsAndIgnoresSecondAnswer()
    {
        var viewModel = await CreateLoadedViewModelAsync();
        var wrong = viewModel.Options.Single(o => o.Text == "Berlin");

        viewModel.AnswerCommand.Execute(wrong);
        viewModel.AnswerCommand.Execute(viewModel.Options.Single(o => o.Text == "Warsaw"));

        Assert.Equal(AnswerState.Wrong, wrong.State);
        Assert.Equal(UiText.QuizScore(0, 1), viewModel.Score);
    }
}
