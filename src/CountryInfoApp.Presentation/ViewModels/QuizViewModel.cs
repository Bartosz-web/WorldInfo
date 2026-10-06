using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;
using CountryInfoApp.Presentation.Localization;

namespace CountryInfoApp.Presentation.ViewModels;

public sealed partial class QuizViewModel : TabViewModelBase
{
    private readonly ICountryCatalogService _catalogService;
    private readonly IQuizService _quizService;
    private IReadOnlyList<CountryDetails> _countries = [];
    private QuizQuestion? _question;

    [ObservableProperty]
    private string _questionText = string.Empty;

    [ObservableProperty]
    private string? _flagUrl;

    [ObservableProperty]
    private bool _isAnswered;

    [ObservableProperty]
    private string? _feedback;

    [ObservableProperty]
    private string _score = UiText.QuizScore(0, 0);

    private int _correctCount;
    private int _answeredCount;

    public QuizViewModel(ICountryCatalogService catalogService, IQuizService quizService)
    {
        _catalogService = catalogService;
        _quizService = quizService;
    }

    public override string Title => UiText.QuizTab;

    public ObservableCollection<QuizOptionViewModel> Options { get; } = [];

    protected override async Task LoadAsync()
    {
        _countries = await _catalogService.GetAllCountryDetailsAsync();
        NextQuestion();
    }

    [RelayCommand]
    private void NextQuestion()
    {
        if (_countries.Count == 0)
        {
            return;
        }

        _question = _quizService.CreateQuestion(_countries);
        QuestionText = _question.Type switch
        {
            QuizQuestionType.Capital => UiText.QuizCapitalQuestion(_question.Country.Name),
            QuizQuestionType.Currency => UiText.QuizCurrencyQuestion(_question.Country.Name),
            _ => UiText.QuizFlagQuestion,
        };
        FlagUrl = _question.Type == QuizQuestionType.Flag ? _question.Country.FlagUrl : null;
        Options.ReplaceWith(_question.Options.Select(text => new QuizOptionViewModel(text)));
        Feedback = null;
        IsAnswered = false;
    }

    // Buttons stay enabled after answering so that their correct/wrong colouring remains visible.
    [RelayCommand]
    private void Answer(QuizOptionViewModel? option)
    {
        if (IsAnswered || _question is null || option is null)
        {
            return;
        }

        var isCorrect = string.Equals(option.Text, _question.CorrectAnswer, StringComparison.OrdinalIgnoreCase);
        foreach (var candidate in Options)
        {
            if (string.Equals(candidate.Text, _question.CorrectAnswer, StringComparison.OrdinalIgnoreCase))
            {
                candidate.State = AnswerState.Correct;
            }
            else if (candidate == option)
            {
                candidate.State = AnswerState.Wrong;
            }
        }

        _answeredCount++;
        _correctCount += isCorrect ? 1 : 0;
        Score = UiText.QuizScore(_correctCount, _answeredCount);
        Feedback = isCorrect ? UiText.QuizCorrect : UiText.QuizWrong(_question.CorrectAnswer);
        IsAnswered = true;
    }

    [RelayCommand]
    private void ResetScore()
    {
        _correctCount = 0;
        _answeredCount = 0;
        Score = UiText.QuizScore(0, 0);
        NextQuestion();
    }
}
