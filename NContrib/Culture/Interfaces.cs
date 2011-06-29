using System.Collections.Generic;

namespace NContrib.Culture {

    public interface IInflector {

        string ToPlural(string word);
        string ToPlural(string word, int number);
        string ToSingular(string word);
        string ToSingular(string word, int number);
    }

    public interface ITextCaseTransformer {

        string ToTitleCase(string input, IEnumerable<string> specials = null);
    }

    public interface ITextGenerator {

        string NumberToWords(decimal number);
        string NumberToWords(int number);
    }
}
