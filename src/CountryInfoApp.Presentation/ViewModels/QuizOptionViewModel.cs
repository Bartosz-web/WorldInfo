using CommunityToolkit.Mvvm.ComponentModel;

namespace CountryInfoApp.Presentation.ViewModels;

public enum AnswerState
{
    NotAnswered,
    Correct,
    Wrong,
}

public sealed partial class QuizOptionViewModel : ObservableObject
{
    [ObservableProperty]
    private AnswerState _state;

    public QuizOptionViewModel(string text)
    {
        Text = text;
    }

    public string Text { get; }
}
