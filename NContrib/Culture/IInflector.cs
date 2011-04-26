namespace NContrib.Culture {

    public interface IInflector {

        string ToPlural(string word);
        string ToPlural(string word, int number);
        string ToSingular(string word);
        string ToSingular(string word, int number);
    }
}
