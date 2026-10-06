using CountryInfoApp.Core.Models;

namespace CountryInfoApp.Core.Abstractions;

public interface IQuizService
{
    QuizQuestion CreateQuestion(IReadOnlyList<CountryDetails> countries);
}
