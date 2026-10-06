using System.Globalization;

namespace CountryInfoApp.Presentation.Localization;

/// <summary>
/// All user-facing texts produced by view models. The application UI is Polish.
/// </summary>
public static class UiText
{
    private static readonly CultureInfo Polish = CultureInfo.GetCultureInfo("pl-PL");

    public const string AppTitle = "Kraje świata – informacje o krajach";

    public const string BrowseTab = "Przeglądaj";
    public const string LookupTab = "Wyszukiwarka";
    public const string CurrenciesTab = "Waluty";
    public const string LanguagesTab = "Języki";
    public const string AllCountriesTab = "Wszystkie kraje";
    public const string StatisticsTab = "Statystyki";
    public const string QuizTab = "Quiz";

    public const string ServiceUnavailable =
        "Nie udało się pobrać danych z usługi. Sprawdź połączenie z internetem i spróbuj ponownie.";
    public const string NotFound = "Nie znaleziono danych dla podanej wartości.";
    public const string FileError = "Nie udało się zapisać pliku. Sprawdź, czy masz uprawnienia do wybranej lokalizacji.";
    public const string Unknown = "brak danych";

    public const string LookupCountryIsoCode = "Kod ISO kraju na podstawie nazwy";
    public const string LookupCountryName = "Nazwa kraju na podstawie kodu ISO";
    public const string LookupCapitalCity = "Stolica kraju";
    public const string LookupPhoneCode = "Numer kierunkowy kraju";
    public const string LookupCountryCurrency = "Waluta kraju";
    public const string LookupCurrencyName = "Nazwa waluty na podstawie kodu";
    public const string LookupLanguageIsoCode = "Kod ISO języka na podstawie nazwy";
    public const string LookupLanguageName = "Nazwa języka na podstawie kodu ISO";

    public const string HintCountryName = "Nazwa kraju po angielsku, np. Poland";
    public const string HintCountryIsoCode = "Kod ISO kraju, np. PL";
    public const string HintCurrencyIsoCode = "Kod waluty, np. EUR";
    public const string HintLanguageName = "Nazwa języka po angielsku, np. Polish";
    public const string HintLanguageIsoCode = "Kod ISO języka, np. pl";

    public const string CsvFileFilter = "Pliki CSV (*.csv)|*.csv";
    public const string CsvDefaultFileName = "kraje.csv";

    public const string QuizFlagQuestion = "Do którego kraju należy ta flaga?";
    public const string QuizCorrect = "Dobrze!";

    public static string QuizCapitalQuestion(string country) => $"Jaka jest stolica kraju {country}?";

    public static string QuizCurrencyQuestion(string country) => $"Jaka waluta obowiązuje w kraju {country}?";

    public static string QuizWrong(string correctAnswer) => $"Niestety, prawidłowa odpowiedź to: {correctAnswer}.";

    public static string QuizScore(int correct, int answered) =>
        string.Format(Polish, "Wynik: {0} / {1}", correct, answered);

    public static string ExportDone(int count, string filePath) =>
        string.Format(Polish, "Wyeksportowano {0} krajów do pliku {1}.", count, filePath);

    public static string CountriesTotal(int count) => string.Format(Polish, "Liczba krajów: {0}", count);

    public static string PhoneCode(string code) => string.IsNullOrWhiteSpace(code) ? Unknown : $"+{code}";

    public static string CodeAndName(string code, string name) => $"{name} ({code})";
}
