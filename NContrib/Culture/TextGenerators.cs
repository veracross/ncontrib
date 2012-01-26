namespace NContrib.Culture {

    public class TextGenerators {

        private static readonly ITextGenerator _english = new EnglishTextGenerator();

        public static ITextGenerator English {
            get { return _english; }
        }
    }
}
