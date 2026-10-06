using CountryInfoApp.Core.Abstractions;
using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Services;

public sealed class QuizService : IQuizService
{
    public const int OptionCount = 4;

    private static readonly QuizQuestionType[] QuestionTypes = Enum.GetValues<QuizQuestionType>();

    private readonly Random _random;

    public QuizService(Random random)
    {
        _random = random;
    }

    public QuizQuestion CreateQuestion(IReadOnlyList<CountryDetails> countries)
    {
        ArgumentNullException.ThrowIfNull(countries);

        var type = QuestionTypes[_random.Next(QuestionTypes.Length)];
        Func<CountryDetails, string> answerOf = AnswerSelector(type);

        var candidates = countries.Where(c => !string.IsNullOrWhiteSpace(answerOf(c))).ToList();
        var distinctAnswers = candidates.Select(answerOf).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        if (distinctAnswers < OptionCount)
        {
            throw new InvalidOperationException($"At least {OptionCount} countries with distinct answers are required.");
        }

        var country = candidates[_random.Next(candidates.Count)];
        var correctAnswer = answerOf(country);

        var options = candidates
            .Select(answerOf)
            .Where(answer => !string.Equals(answer, correctAnswer, StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(_ => _random.Next())
            .Take(OptionCount - 1)
            .Append(correctAnswer)
            .OrderBy(_ => _random.Next())
            .ToList();

        return new QuizQuestion(type, country, options, correctAnswer);
    }

    private static Func<CountryDetails, string> AnswerSelector(QuizQuestionType type) => type switch
    {
        QuizQuestionType.Capital => c => c.CapitalCity,
        QuizQuestionType.Flag => c => c.Name,
        QuizQuestionType.Currency => c => c.CurrencyIsoCode,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
    };
}
