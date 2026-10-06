using CountryInfoApp.Core.Models;
using CountryInfoApp.Core.Services;
using CountryInfoApp.Tests.Fakes;

namespace CountryInfoApp.Tests.Core;

public class QuizServiceTests
{
    private static readonly CountryDetails[] Countries =
    [
        TestData.Country("PL", "Poland", "Warsaw", currency: "PLN"),
        TestData.Country("DE", "Germany", "Berlin", currency: "EUR"),
        TestData.Country("FR", "France", "Paris", currency: "EUR"),
        TestData.Country("CZ", "Czechia", "Prague", currency: "CZK"),
        TestData.Country("GB", "United Kingdom", "London", currency: "GBP"),
        TestData.Country("US", "United States", "Washington", currency: "USD"),
    ];

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(42)]
    public void CreateQuestion_ReturnsFourDistinctOptionsIncludingCorrectAnswer(int seed)
    {
        var service = new QuizService(new Random(seed));

        var question = service.CreateQuestion(Countries);

        Assert.Equal(QuizService.OptionCount, question.Options.Count);
        Assert.Equal(question.Options.Count, question.Options.Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.Contains(question.CorrectAnswer, question.Options);
    }

    [Fact]
    public void CreateQuestion_CorrectAnswerMatchesCountry()
    {
        var service = new QuizService(new Random(7));

        for (var i = 0; i < 20; i++)
        {
            var question = service.CreateQuestion(Countries);
            var expected = question.Type switch
            {
                QuizQuestionType.Capital => question.Country.CapitalCity,
                QuizQuestionType.Flag => question.Country.Name,
                _ => question.Country.CurrencyIsoCode,
            };
            Assert.Equal(expected, question.CorrectAnswer);
        }
    }

    [Fact]
    public void CreateQuestion_Throws_WhenNotEnoughCountries()
    {
        var service = new QuizService(new Random(1));

        Assert.Throws<InvalidOperationException>(() => service.CreateQuestion(Countries.Take(2).ToList()));
    }
}
