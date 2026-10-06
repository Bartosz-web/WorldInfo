namespace CountryInfoApp.Core.Models;

public enum QuizQuestionType
{
    Capital,
    Flag,
    Currency,
}

/// <summary>
/// A language-neutral quiz question; the presentation layer decides how to phrase it.
/// </summary>
public sealed record QuizQuestion(
    QuizQuestionType Type,
    CountryDetails Country,
    IReadOnlyList<string> Options,
    string CorrectAnswer);
