namespace NContrib.Culture {

    public class Inflectors {

        private static IInflector _english;

        public static IInflector English { get { return _english ?? (_english = new EnglishInflector()); } }
    }
}
