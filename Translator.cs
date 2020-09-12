using System.Collections.Generic;

public class Translator
{
    static Dictionary<string, Dictionary<string, string>> translations = new Dictionary<string, Dictionary<string, string>>()
    {
        { "Yes", new Dictionary<string, string>(){{ "fr","Oui" }}},
        { "No", new Dictionary<string, string>(){{ "fr","Non" }}},
        { "Cancel", new Dictionary<string, string>(){{ "fr","Annuler" }}},
        { "Retry", new Dictionary<string, string>(){{ "fr","Réessayer" }}}
    };

    public static string Translate(string input, string culture)
    {
        if (!translations.ContainsKey(input))
            return input;
        if (translations[input].ContainsKey(culture))
            return translations[input][culture];
        else
            return input;
    }
}

